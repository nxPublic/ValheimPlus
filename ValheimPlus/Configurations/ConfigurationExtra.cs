﻿using BepInEx;
using IniParser;
using IniParser.Model;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IniParser.Parser;
using UnityEngine;

namespace ValheimPlus.Configurations
{
    // Configuration implementation part
    public partial class Configuration
    {
        public static string ConfigIniPath = Path.GetDirectoryName(Paths.BepInExConfigPath);

        // Loaded configuration
        public static Configuration Current { get; private set; }

        /// <summary>
        /// Load configuration from it's ini files
        /// </summary>
        /// <param name="isClient"></param>
        /// <returns>true if successful</returns>
        public static bool LoadConfiguration(bool isClient = false)
        {
            bool errorWhileLoadingIni = false;
            Current = new Configuration();
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(Configuration).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    string iniPath = Path.Combine(ConfigIniPath, property.Name + ".ini");

                    bool needsSync = typeof(ISyncableSection).IsAssignableFrom(property.PropertyType);

                    if ((isClient && !needsSync) || (!isClient && needsSync))
                    {
                        if (File.Exists(iniPath))
                        {
                            errorWhileLoadingIni |= !LoadFromIni(Current, property, iniPath, sb);
                        }
                        else
                        {
                            Debug.Log($"Saving missing default config for {property.Name}");
                            Current.SaveConfiguration(property, isClient);
                        }
                    }
                }

                // If there were errors while loading the ini files, try to write missing ones
                if (errorWhileLoadingIni)
                {
                    Debug.Log("Error(s) occured loading configuration files");
                    Debug.Log(sb.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }

            return !errorWhileLoadingIni;
        }

        public Configuration()
        {
            // Create all configuration entries with default values
            foreach (var property in typeof(Configuration).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.SetValue(this, Activator.CreateInstance(property.PropertyType, true), null);
            }
        }

        public string GetSyncableSections()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var property in typeof(Configuration).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                sb.AppendLine(GenerateSection(property, property.GetValue(this, null), false));
            }

            return sb.ToString();
        }


        /// <summary>
        /// Save full config
        /// Do not call this from startup, ZNet isn't initialized yet
        /// </summary>
        public void SaveConfiguration()
        {
            bool isClient = !ZNet.instance.IsServer();
            foreach (var property in typeof(Configuration).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList())
            {
                SaveConfiguration(property, isClient);
            }
        }

        private void SaveConfiguration(PropertyInfo property, bool isClient)
        {
            string configName = property.Name + ".ini";

            Type sectionType = property.PropertyType;

            // For clients only save client values, for servers only server values
            object section = property.GetValue(Configuration.Current, null);
            bool needsSync = section is ISyncableSection;

            if ((isClient && !needsSync) || (!isClient && needsSync))
            {
                using (TextWriter tw = new StreamWriter(Path.Combine(ConfigIniPath, configName)))
                {
                    tw.Write(GenerateSection(property, section, true));
                }
            }
        }

        private static string GenerateSection(PropertyInfo property, object section, bool withComments = false)
        {
            StringBuilder sb = new StringBuilder();
            Type sectionType = property.PropertyType;
            var sectionAttribute = sectionType.GetCustomAttributes(false).OfType<ConfigurationSectionAttribute>().FirstOrDefault();

            if (sectionAttribute != null && withComments)
            {
                sb.AppendLine(sectionAttribute.Comment.ToIniComment());
                sb.AppendLine("; V" + sectionAttribute.SinceVersion);
            }

            sb.AppendLine($"[{property.Name}]");

            // IsEnabled first

            var enabledProperty = typeof(IConfig).GetProperty("IsEnabled", BindingFlags.Public | BindingFlags.Instance);
            var enabledValue = enabledProperty.GetValue(section, null);
            string enabledAsString = enabledValue.ToString();

            if (withComments)
            {
                sb.AppendLine("; Change false to true to enable this section");
            }
            sb.AppendLine($"{enabledProperty.Name}={enabledAsString}");

            if (withComments)
            {
                sb.AppendLine();
            }

            foreach (var configProperty in sectionType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).ToList())
            {
                // Dont need to write NeedsServerSync

                if (configProperty.Name == "NeedsServerSync")
                {
                    continue;
                }

                var value = configProperty.GetValue(section, null);
                string valueAsString = value.ToString();
                if (value is float)
                {
                    valueAsString = ((float) value).ToString(CultureInfo.InvariantCulture.NumberFormat);
                }

                // Special case 'IsEnabled'
                if (configProperty.Name == "IsEnabled")
                {
                    continue;
                }

                ConfigurationAttribute ca = configProperty.GetCustomAttributes(false).OfType<ConfigurationAttribute>().FirstOrDefault();

                if (ca != null && withComments)
                {
                    sb.Append(ca.Comment.ToIniComment());
                    sb.AppendLine($"; V{ca.SinceVersion}");
                }

                sb.AppendLine($"{configProperty.Name}={valueAsString}");

                if (withComments)
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public static bool LoadFromIni(Configuration config, PropertyInfo property, string filename, StringBuilder sb)
        {
            try
            {
                var parser = new FileIniDataParser();

                IniData configdata = new IniData();
                if (File.Exists(filename))
                {
                    configdata = parser.ReadFile(filename);
                }

                var keyName = property.Name;
                var method = property.PropertyType.GetMethod("LoadIni", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                if (method != null)
                {
                    var result = method.Invoke(null, new object[] { configdata, keyName });
                    property.SetValue(config, result, null);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error loading ini file {filename}");
                Debug.Log(e.Message);
                sb.AppendLine($"Error loading ini file {filename}");
                sb.AppendLine($"{e.Message}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Only set values which should be synced to current configuration
        /// </summary>
        /// <param name="receivedConfig"></param>
        public static void SetSyncableValues(Configuration receivedConfig)
        {
            foreach (var property in typeof(Configuration).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(ISyncableSection).IsAssignableFrom(x.PropertyType)))
            {
                property.SetValue(Current, property.GetValue(receivedConfig, null), null);
            }
        }

        public static void LoadFromIniString(Configuration receivedConfig, string inputString)
        {
            try
            {
                var parser = new IniDataParser();
                var ini = parser.Parse(inputString);

                foreach (var property in typeof(Configuration).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (ini.Sections.ContainsSection(property.Name))
                    {
                        var result = property.PropertyType.GetMethod("LoadIni", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?
                            .Invoke(null, new object[] { ini, property.Name });
                        if (result == null)
                        {
                            throw new Exception($"LoadIni method not found on Type {property.PropertyType.Name}");
                        }
                        property.SetValue(receivedConfig, result, null);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error {e.Message} occured while parsing synced config");

                Debug.LogError($"while parsing {Environment.NewLine}{inputString}");
            }
        }

    }

    public static class StringExtensions
    {
        public static string ToIniComment(this string comment)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string line in comment.Split('\n'))
            {
                sb.AppendLine("; " + line);
            }

            return sb.ToString();
        }
    }


    public static class IniDataExtensions
    {
        public static float GetFloat(this KeyDataCollection data, string key, float defaultVal)
        {
            if (float.TryParse(data[key], NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out var result))
            {
                return result;
            }
            Debug.LogWarning($" [Float] Could not read {key}, using default value of {defaultVal}");
            return defaultVal;
        }
        public static bool GetBool(this KeyDataCollection data, string key)
        {
            var truevals = new[] { "y", "yes", "true" };
            return truevals.Contains(data[key].ToLower());
        }
        public static int GetInt(this KeyDataCollection data, string key, int defaultVal)
        {
            if (int.TryParse(data[key], NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out var result))
            {
                return result;
            }
            Debug.LogWarning($" [Int] Could not read {key}, using default value of {defaultVal}");
            return defaultVal;
        }

        public static KeyCode GetKeyCode(this KeyDataCollection data, string key, KeyCode defaultVal)
        {
            if (System.Enum.TryParse<KeyCode>(data[key], out var result))
            {
                return result;
            }
            Debug.LogWarning($" [KeyCode] Could not read {key}, using default value of {defaultVal}");
            return defaultVal;
        }

        public static T LoadConfiguration<T>(this IniData data, string key) where T : BaseConfig<T>, new()
        {
            // this function gives null reference error
            var idata = data[key];
            return (T)typeof(T).GetMethod("LoadIni").Invoke(null, new[] { idata });
        }

    }
}

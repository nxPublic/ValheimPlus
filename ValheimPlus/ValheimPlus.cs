﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using BepInEx;
using HarmonyLib;
using ValheimPlus.Configurations;
using ValheimPlus.RPC;
using ValheimPlus.UI;

namespace ValheimPlus
{
    // COPYRIGHT 2021 KEVIN "nx#8830" J. // http://n-x.xyz
    // GITHUB REPOSITORY https://github.com/valheimPlus/ValheimPlus
    

    [BepInPlugin("org.bepinex.plugins.valheim_plus", "Valheim Plus", version)]
    public class ValheimPlusPlugin : BaseUnityPlugin
    {
        public const string version = "0.9.6";
        public static string newestVersion = "";
        public static bool isUpToDate = false;

        public static System.Timers.Timer mapSyncSaveTimer =
            new System.Timers.Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);

        public static readonly string VPlusDataDirectoryPath =
            Paths.BepInExRootPath + Path.DirectorySeparatorChar + "vplus-data";

        public static Harmony harmony = new Harmony("mod.valheim_plus");

        // Project Repository Info
        public static string Repository = "https://github.com/valheimPlus/ValheimPlus";
        public static string ApiRepository = "https://api.github.com/repos/valheimPlus/valheimPlus/tags";

        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            Logger.LogInfo("Trying to load the configuration file");

            if (ConfigurationExtra.LoadSettings() != true)
            {
                Logger.LogError("Error while loading configuration file.");
            }
            else
            {
                Logger.LogInfo("Configuration file loaded succesfully.");

                
                harmony.PatchAll();

                if (IsNewVersionAvailable() == "new")
                {
                    Logger.LogError("There is a newer version available of ValheimPlus.");
                    Logger.LogWarning("Please visit " + ValheimPlusPlugin.Repository + ".");
                } 
                else if (IsNewVersionAvailable() == "same")
                {
                    Logger.LogInfo("ValheimPlus [" + version + "] is up to date.");
                } 
                else if (IsNewVersionAvailable() == "old")
                {
                    Logger.LogError("You are in a version ahead of the most current one.");
                    Logger.LogWarning("If you are not a developer, please switch back to the most current stable version published.");
                    Logger.LogWarning("Please visit " + ValheimPlusPlugin.Repository + ".");
                } 
                else if (IsNewVersionAvailable() == "fail")
                {
                    Logger.LogError("There was a fail in stipulating the version.");
                    Logger.LogWarning("Please visit " + ValheimPlusPlugin.Repository + ".");
                }
                else
                {
                    Logger.LogError("There was a fail in stipulating the version.");
                    Logger.LogWarning("Please visit " + ValheimPlusPlugin.Repository + ".");
                }

                //Create VPlus dir if it does not exist.
                if (!Directory.Exists(VPlusDataDirectoryPath)) Directory.CreateDirectory(VPlusDataDirectoryPath);

                //Logo
                if(Configuration.Current.ValheimPlus.IsEnabled && Configuration.Current.ValheimPlus.mainMenuLogo)
                    VPlusMainMenu.Load();

                //Map Sync Save Timer
                if (ZNet.m_isServer && Configuration.Current.Map.IsEnabled && Configuration.Current.Map.shareMapProgression)
                {
                    mapSyncSaveTimer.AutoReset = true;
                    mapSyncSaveTimer.Elapsed += (sender, args) => VPlusMapSync.SaveMapDataToDisk();
                }
            }
        }

        public static string IsNewVersionAvailable()
        {
            WebClient client = new WebClient();

            client.Headers.Add("User-Agent: V+ Server");

            try
            {
                string reply = client.DownloadString(ApiRepository);
                newestVersion = reply.Split(new[] { "," }, StringSplitOptions.None)[0].Trim().Replace("\"", "").Replace("[{name:", "");
            }
            catch
            {
                ZLog.Log("The newest version could not be determined.");
                newestVersion = "Unknown";
            }

            //Parse versions for proper version check
            if (System.Version.TryParse(newestVersion, out System.Version newVersion))
            {
                if (System.Version.TryParse(version, out System.Version currentVersion))
                {
                    if (currentVersion < newVersion && (currentVersion != newVersion))
                    {
                        return "new";
                    }
                    if (currentVersion > newVersion && (currentVersion != newVersion))
                    {
                        return "old";
                    }
                    if (currentVersion == newVersion)
                    {
                        return "same";
                    }
                }
            }
            else //Fallback version check if the version parsing fails
            {
                if (newestVersion != version)
                {
                    return "fail";
                }
            }
            
            return "fail";
        }
    }
}

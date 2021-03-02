﻿using BepInEx;
using HarmonyLib;
using ValheimPlus.Configurations;
using ValheimPlus.UI;

namespace ValheimPlus
{
    // COPYRIGHT 2021 KEVIN "nx#8830" J. // http://n-x.xyz
    // GITHUB REPOSITORY https://github.com/valheimPlus/ValheimPlus
    

    [BepInPlugin("org.bepinex.plugins.valheim_plus", "Valheim Plus", "0.9.2.3")]
    class ValheimPlusPlugin : BaseUnityPlugin
    {
        
        public static string version = "0.9.2.2";
        public static string newestVersion = "";
        public static bool isUpToDate = false;

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

                Harmony harmony = new Harmony("mod.valheim_plus");
                harmony.PatchAll();
                /*
                                isUpToDate = !Settings.isNewVersionAvailable();
                                if (!isUpToDate)
                                {
                                    Logger.LogError("There is a newer version available of ValheimPlus.");
                                    Logger.LogWarning("Please visit " + ValheimPlusPlugin.Repository + ".");
                                }
                                else
                                {
                                    Logger.LogInfo("ValheimPlus [" + version + "] is up to date.");
                                }
                */
                Logger.LogInfo("ValheimPlus [" + version + "] is up to date.");

                //Logo
                VPlusMainMenu.Load();
            }
        }
    }
}

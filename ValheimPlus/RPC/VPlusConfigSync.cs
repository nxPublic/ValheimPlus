﻿using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using ValheimPlus.Configurations;

namespace ValheimPlus.RPC
{
    public class VPlusConfigSync
    {

        static public bool isConnecting = false;
        public static void RPC_VPlusConfigSync(long sender, ZPackage configPkg)
        {
            if (ZNet.m_isServer) //Server
            {
                if (!Configuration.Current.Server.IsEnabled || !Configuration.Current.Server.serverSyncsConfig) return;

                ZPackage pkg = new ZPackage();

                string[] rawConfigData = File.ReadAllLines(ConfigurationExtra.ConfigIniPath);
                List<string> cleanConfigData = new List<string>();

                for (int i = 0; i < rawConfigData.Length; i++)
                {
                    if (rawConfigData[i].Trim().StartsWith(";") ||
                        rawConfigData[i].Trim().StartsWith("#")) continue; //Skip comments

                    if (rawConfigData[i].Trim().IsNullOrWhiteSpace()) continue; //Skip blank lines

                    //Add to clean data
                    cleanConfigData.Add(rawConfigData[i]);
                }

                //Add number of clean lines to package
                pkg.Write(cleanConfigData.Count);

                //Add each line to the package
                foreach (string line in cleanConfigData)
                {
                    pkg.Write(line);
                }

                // Sync the recipe manager if it's enabled
                if (RecipeManager.instance != null)
                {
                    RecipeManager.instance.Config.Serialize(pkg);
                }

                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "VPlusConfigSync", new object[]
                {
                    pkg
                });

                ZLog.Log("VPlus configuration synced to peer #" + sender);
            }
            else //Client
            {
                if (configPkg != null && 
                    configPkg.Size() > 0 && 
                    sender == ZRoutedRpc.instance.GetServerPeerID()) //Validate the message is from the server and not another client.
                {
                    int numLines = configPkg.ReadInt();

                    if (numLines == 0)
                    {
                        ZLog.LogWarning("Got zero line config file from server. Cannot load.");
                        return;
                    }

                    using (MemoryStream memStream = new MemoryStream())
                    {
                        using (StreamWriter tmpWriter = new StreamWriter(memStream))
                        {
                            for (int i = 0; i < numLines; i++)
                            {
                                string line = configPkg.ReadString();

                                tmpWriter.WriteLine(line);
                            }

                            tmpWriter.Flush(); //Flush to memStream
                            memStream.Position = 0; //Rewind stream

                            // Sync the recipe manager if it's enabled

                            ValheimPlusPlugin.harmony.UnpatchSelf();

                            // Sync HotKeys when connecting ?
                            if(Configuration.Current.Server.IsEnabled && !Configuration.Current.Server.serverSyncHotkeys)
                            {
                                isConnecting = true;
                                Configuration.Current = ConfigurationExtra.LoadFromIni(memStream);
                                isConnecting = false;
                            }
                            else
                            {
                                Configuration.Current = ConfigurationExtra.LoadFromIni(memStream);
                            }
                                
                            ValheimPlusPlugin.harmony.PatchAll();

                            if (RecipeManager.instance != null)
                            {
                                try
                                {
                                    RecipeManager.instance.Config.Unserialize(configPkg);
                                    RecipeManager.instance.SyncRecipes();
                                } 
                                catch(System.Exception e)
                                {
                                    ZLog.LogError($"Error syncing recipe manager configuration: {e.Message}");
                                }                                
                            }

                            ZLog.Log("Successfully synced VPlus configuration from server.");
                        }
                    }
                }
            }
        }
    }
}
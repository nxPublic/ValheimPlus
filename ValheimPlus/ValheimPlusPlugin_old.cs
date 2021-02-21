﻿using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using UnityEngine;
using System.IO;
using System.Reflection;
using HarmonyLib;
using System.Globalization;
using Steamworks;

namespace ValheimPlus
{
    // COPYRIGHT 2021 KEVIN "nx#8830" J. // http://n-x.xyz
    // GITHUB REPOSITORY https://github.com/nxPublic/ValheimPlus


    [BepInPlugin("org.bepinex.plugins.valheim_plus", "Valheim Plus", "0.8.0.0")]
    class ValheimPlusPluginOld : BaseUnityPlugin
    {
        string ConfigYamlPath = Path.GetDirectoryName(Paths.BepInExConfigPath) + "\\valheim_plus.yml";
        string ConfigIniPath = Path.GetDirectoryName(Paths.BepInExConfigPath) + "\\valheim_plus.ini";

        // DO NOT REMOVE MY CREDITS
        string Author = "nx";
        string Website = "http://n-x.xyz";
        string Discord = "nx#8830";
        string Repository = "https://github.com/nxPublic/ValheimPlus";

        // Add your credits here
        String ModifiedBy = "YourName";

        public static Boolean isDebug = false;

        public static Configuration Conf { get; set; }

        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            Logger.LogInfo("Trying to load the configuration file");

            if (LoadSettings() != true)
            {
                Logger.LogError("Error while loading configuration file.");
            }
            else
            {
                Logger.LogInfo("Configuration file loaded succesfully.");
                // apply hooks
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            }
            
        }
        private bool LoadSettings()
        {
            try
            {
                if (File.Exists(ConfigYamlPath))
                    Conf = ConfigurationExtra.LoadFromYaml(ConfigYamlPath);
                else if (File.Exists(ConfigIniPath))
                    Conf = ConfigurationExtra.LoadFromIni(ConfigIniPath);
                else
                {
                    Logger.LogError("Error: Configuration not found. Plugin not loaded.");
                    return false;
                }
                //var parser = new FileIniDataParser();
                //Config = parser.ReadFile(ConfigPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Could not load config file: {ex}");
                return false;
            }
            return true;
        }

        // ##################################################### SECTION = PLAYER
        [HarmonyPatch(typeof(Player), "GetMaxCarryWeight")]
        public static class ModifyMaximumCarryWeight
        {
            private static void Postfix(ref float __result)
            {
                if(Conf.Player != null)
                {
                    bool Megingjord = false;
                    float carryWeight = __result;

                    if (carryWeight > 300)
                    {
                        Megingjord = true;
                        carryWeight -= 150;
                    }

                    carryWeight = Conf.Player.BaseMaximumWeight;// toFloat(Config["Player"]["baseMaximumWeight"]);
                    if (Megingjord)
                    {
                        carryWeight = carryWeight +  Conf.Player.BaseMegingjordBuff;//toFloat(Config["Player"]["baseMegingjordBuff"]);
                    }

                    __result = carryWeight;
                }
            }
        }
        [HarmonyPatch(typeof(Player), "AutoPickup")]
        public static class ModifyAutoPickUpRange
        {
            private static bool Prefix(ref float ___m_autoPickupRange)
            {
                if (Conf.Player != null)
                {
                    ___m_autoPickupRange = Conf.Player.BaseAutoPickUpRange;//toFloat(Config["Player"]["baseAutoPickUpRange"]);
                }
                return true;
            }
        }

        // ##################################################### SECTION = BEEHIVES
        [HarmonyPatch(typeof(Beehive), "Awake")]
        public static class ApplyBeehiveChanges
        {
            private static bool Prefix(ref float ___m_secPerUnit, ref int ___m_maxHoney)
            {
                if (Conf.Beehive != null)
                {
                    ___m_secPerUnit = Conf.Beehive.HoneyProductionSpeed; //float.Parse(Config["Beehive"]["honeyProductionSpeed"]);
                    ___m_maxHoney = Conf.Beehive.MaximumHoneyPerBeehive;//int.Parse(Config["Beehive"]["maximumHoneyPerBeehive"]);
                    if (isDebug)
                    {
                        Debug.Log($"Beehive Production : {Conf.Beehive.HoneyProductionSpeed}");
                        Debug.Log($"Beehive Maximum : {Conf.Beehive.MaximumHoneyPerBeehive}");
                    }
                }
                return true;
            }

        }

        // ##################################################### SECTION = Fermenter
        [HarmonyPatch(typeof(Fermenter), "Awake")]
        public static class ApplyFermenterChanges
        {
            private static bool Prefix(ref float ___m_fermentationDuration, ref Fermenter __instance)
            {
                if (Conf.Fermenter != null)
                {
                    float fermenterDuration = Conf.Fermenter.FermenterDuration;//toFloat(Config["Fermenter"]["fermenterDuration"]);
                    if (fermenterDuration > 0)
                    {
                        ___m_fermentationDuration = fermenterDuration;
                    }
                }
                return true;
            }

        }
        [HarmonyPatch(typeof(Fermenter), "GetItemConversion")]
        public static class ApplyFermenterItemCountChanges
        {
            private static void Postfix(ref Fermenter.ItemConversion __result)
            {
                if (Conf.Fermenter != null)
                {
                    int fermenterItemCount = Conf.Fermenter.FermenterItemsProduced;
                    if (fermenterItemCount > 0)
                    {
                        __result.m_producedItems = fermenterItemCount;
                    }
                }
                    
            }

        }


        // ##################################################### SECTION = Items
        [HarmonyPatch(typeof(Inventory), "IsTeleportable")]
        public static class noItemTeleportPrevention
        {
            private static void Postfix(ref Boolean __result)
            {
                if (Conf.Items != null)
                {
                    if (Conf.Items.NoTeleportPrevention)
                    __result = true;
                }
            }
        }

        
        // ##################################################### SECTION = Furnace
        [HarmonyPatch(typeof(Smelter), "Awake")]
        public static class ApplyFurnaceChanges
        {
            private static void Prefix(ref Smelter __instance)
            {
                if (Conf.Furnace != null)
                {

                    if (!__instance.m_addWoodSwitch && Conf.Kiln != null)
                    {
                       float ProductionSpeed_k = Conf.Kiln.ProductionSpeed;//toFloat(Config["Kiln"]["productionSpeed"]);

                        __instance.m_secPerProduct = ProductionSpeed_k;
                    }
                    else
                    {
                        // is furnace
                        __instance.m_maxOre = Conf.Furnace.MaximumOre;
                        __instance.m_maxFuel = Conf.Furnace.MaximumCoal;
                        __instance.m_secPerProduct = Conf.Furnace.ProductionSpeed;
                        __instance.m_fuelPerProduct = Conf.Furnace.CoalUsedPerProduct;
                    }
                }
            }
        }

        // ##################################################### SECTION = Tooltip & Item Modification
        [HarmonyPatch(typeof(ItemDrop), "Awake")]
        public static class ChangeTooltip
        {
            private static void Prefix(ref ItemDrop __instance)
            {
                
                if (Conf.Items != null && Conf.Items.NoTeleportPrevention)
                {
                    __instance.m_itemData.m_shared.m_teleportable = true;
                }

                if (Conf.Food != null)
                {
                    float food_multiplier = Conf.Food.FoodDurationMultiplier;
                    if (food_multiplier > 0.1)
                    {
                        if (Convert.ToInt32(__instance.m_itemData.m_shared.m_itemType) == 2)
                            __instance.m_itemData.m_shared.m_foodBurnTime = __instance.m_itemData.m_shared.m_foodBurnTime + (__instance.m_itemData.m_shared.m_foodBurnTime * Conf.Food.FoodDurationMultiplier);
                    }
                }


                if (Conf.Items != null)
                {
                    float itemWeigthReduction = Conf.Items.BaseItemWeightReduction;
                    if (itemWeigthReduction > 0)
                    {
                        __instance.m_itemData.m_shared.m_weight = __instance.m_itemData.m_shared.m_weight - (__instance.m_itemData.m_shared.m_weight * itemWeigthReduction);
                    }
                }

            }
        }



        // ##################################################### SECTION = BUILDING
        [HarmonyPatch(typeof(Player), "UpdatePlacementGhost")]
        public static class ModifyPlacingRestrictionOfGhost
        {


            static Boolean DelayedStop = false;
            static Boolean BlockRefresh = false;
            static Boolean controlFlag = false;
            static Boolean shiftFlag = false;
            static Boolean altFlag = false;
            
            private static Boolean Prefix(ref Player __instance, ref Int32 ___m_placementStatus, ref GameObject ___m_placementGhost, ref GameObject ___m_placementMarkerInstance, ref int ___m_placeRotation)
            {

                if(Conf.AdvancedBuildingMode != null)
                {
                    return true;
                }

                KeyCode enter = Conf.AdvancedBuildingMode.EnterAdvancedBuildingMode;
                KeyCode exit = Conf.AdvancedBuildingMode.ExitAdvancedBuildingMode;

                // Error Handling and removal of left over placement marker
                if (!__instance.InPlaceMode() || ___m_placementGhost == null)
                {
                    DelayedStop = false;
                    BlockRefresh = false;
                    ___m_placementMarkerInstance.SetActive(false);
                    return true;
                }

                // Delayed function stop to place the object at the right location (if we would immediatly stop, it would be placed at cursor location)
                if (DelayedStop)
                {
                    DelayedStop = false;
                    BlockRefresh = false;
                    return true;
                }

                float rX = 0;
                float rZ = 0;
                float rY = 0;

                Piece component = ___m_placementGhost.GetComponent<Piece>();

                if (Input.GetKeyDown(enter))
                {
                    BlockRefresh = true;
                }
                if (Input.GetKeyDown(exit))
                {
                    BlockRefresh = false;
                    ___m_placeRotation = 0;
                }

                float distance = 2;
                float scrollDistance = 2;

                // TODO ADD INCREASE / DECREASE HOTKEYS
                // TODO ADD HOTKEY TO SAVE / LOAD ROTATION
                
                if (Input.GetKeyDown(KeyCode.LeftControl)){controlFlag = true;}
                if (Input.GetKeyUp(KeyCode.LeftControl)){controlFlag = false;}

                if (Input.GetKeyDown(KeyCode.LeftShift)) { shiftFlag = true; }
                if (Input.GetKeyUp(KeyCode.LeftShift)) { shiftFlag = false; }
                if (shiftFlag){distance = 6; scrollDistance = 6; } else{distance = 2; scrollDistance=2;}

                if (Input.GetKeyDown(KeyCode.LeftAlt)) { altFlag = true; }
                if (Input.GetKeyUp(KeyCode.LeftAlt)) { altFlag = false; }

                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    
                    Quaternion rotation;
                    if (controlFlag)
                    {
                        rX++;
                        rotation = Quaternion.Euler(component.transform.eulerAngles.x + (scrollDistance * (float)rX), component.transform.eulerAngles.y, component.transform.eulerAngles.z); // forward to backwards
                    }
                    else
                    {
                        if (altFlag)
                        {
                            rZ++;
                            rotation = Quaternion.Euler(component.transform.eulerAngles.x, component.transform.eulerAngles.y, component.transform.eulerAngles.z + (scrollDistance * (float)rZ)); // diagonal
                        }
                        else
                        {
                            rY++;
                            rotation = Quaternion.Euler(component.transform.eulerAngles.x, component.transform.eulerAngles.y + (scrollDistance * (float)rY), component.transform.eulerAngles.z); // left<->right
                        }
                    }
                    component.transform.rotation = rotation;
                }
                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    Quaternion rotation;
                    if (controlFlag)
                    {
                        rX--;
                        rotation = Quaternion.Euler(component.transform.eulerAngles.x + (scrollDistance * (float)rX), component.transform.eulerAngles.y, component.transform.eulerAngles.z); // forward to backwards
                    }
                    else
                    {
                        if (altFlag)
                        {
                            rZ--;
                            rotation = Quaternion.Euler(component.transform.eulerAngles.x, component.transform.eulerAngles.y, component.transform.eulerAngles.z + (scrollDistance * (float)rZ)); // diagonal
                        }
                        else
                        {
                            rY--;
                            rotation = Quaternion.Euler(component.transform.eulerAngles.x, component.transform.eulerAngles.y + (scrollDistance * (float)rY), component.transform.eulerAngles.z); // left<->right
                        }
                    }

                    component.transform.rotation = rotation;
                }
                

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (controlFlag)
                    {
                        component.transform.Translate(Vector3.up * distance * Time.deltaTime);
                    }
                    else
                    {
                        component.transform.Translate(Vector3.forward * distance * Time.deltaTime);
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (controlFlag)
                    {
                        component.transform.Translate(Vector3.down * distance * Time.deltaTime);
                    }
                    else
                    {
                        component.transform.Translate(Vector3.back * distance * Time.deltaTime);
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    component.transform.Translate(Vector3.left * distance * Time.deltaTime);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    component.transform.Translate(Vector3.right * distance * Time.deltaTime);
                }


                // ToDo, add a custom additional downwards raycast from the position of the component to the ground to add the rest of the placement restrictions

                bool water = component.m_waterPiece || component.m_noInWater;
                
                ___m_placementStatus = 0; // VALID

                if (component.m_groundOnly || component.m_groundPiece || component.m_cultivatedGroundOnly)
                {
                    ___m_placementMarkerInstance.SetActive(false);
                }

                StationExtension component2 = component.GetComponent<StationExtension>();

                if (component2 != null)
                {
                    CraftingStation craftingStation = component2.FindClosestStationInRange(component.transform.position);
                    if (craftingStation)
                    {
                        component2.StartConnectionEffect(craftingStation);
                    }
                    else
                    {
                        component2.StopConnectionEffect();
                        ___m_placementStatus = 7; // Missing Station
                    }
                    if (component2.OtherExtensionInRange(component.m_spaceRequirement))
                    {
                       ___m_placementStatus = 5; // More Space
                    }
                }

                if (component.m_onlyInTeleportArea && !EffectArea.IsPointInsideArea(component.transform.position, EffectArea.Type.Teleport, 0f))
                {
                    ___m_placementStatus = 6;
                }
                if (!component.m_allowedInDungeons && (component.transform.position.y > 3000f))
                {
                    ___m_placementStatus = 10;
                }
                if (Location.IsInsideNoBuildLocation(___m_placementGhost.transform.position))
                {
                    ___m_placementStatus = 3;
                }
                float radius = component.GetComponent<PrivateArea>() ? component.GetComponent<PrivateArea>().m_radius : 0f;
                if (!PrivateArea.CheckAccess(___m_placementGhost.transform.position, radius, true))
                {
                    ___m_placementStatus = 4;
                }

                if(___m_placementStatus != 0)
                {
                    component.SetInvalidPlacementHeightlight(true);
                }
                else
                {
                    component.SetInvalidPlacementHeightlight(false);
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // Stop with the next iteration of the function and place the piece
                    DelayedStop = true;
                }

                return !BlockRefresh;

            }

            private static void Postfix(ref Player __instance,ref Int32 ___m_placementStatus, ref GameObject ___m_placementGhost, ref GameObject ___m_placementMarkerInstance)
            {

                if (!__instance.InPlaceMode() || ___m_placementGhost == null)
                {
                    if (___m_placementMarkerInstance)
                    {
                        ___m_placementMarkerInstance.SetActive(false);
                    }
                }

                if (Conf.Building != null && Conf.Building.NoInvalidPlacementRestriction)
                {
                    if (___m_placementStatus == 1)
                    {
                        ___m_placementStatus = 0;
                        ___m_placementGhost.GetComponent<Piece>().SetInvalidPlacementHeightlight(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(WearNTear), "ApplyDamage")]
        public static class RemoveWearNTear
        {
            private static Boolean Prefix()
            {
                if (Conf.Building != null && Conf.Building.NoWeatherDamage)
                {
                    return false;
                }
                return true;
            }
        }


        
        // ##################################################### SECTION = SERVER
        [HarmonyPatch(typeof(ZNet), "Awake")]
        public static class ChangeGameServerVariables
        {
            private static void Postfix(ref ZNet __instance) 
            {
                if(Conf.Server != null)
                {
                    int maxPlayers = Conf.Server.MaxPlayers;
                    if (maxPlayers >= 1)
                    {
                        // Set Server Instance Max Players
                        __instance.m_serverPlayerLimit = maxPlayers;
                    }
                }
                
            }
            
        }
        [HarmonyPatch(typeof(SteamGameServer), "SetMaxPlayerCount")]
        public static class ChangeSteamServerVariables
        {
            private static void Prefix(ref int cPlayersMax) 
            {
                if (Conf.Server != null)
                {
                    int maxPlayers = Conf.Server.MaxPlayers;
                    if (maxPlayers >= 1)
                    {
                        cPlayersMax = maxPlayers;
                    }
                }
                
            }

        }
        [HarmonyPatch(typeof(FejdStartup), "IsPublicPasswordValid")]
        public static class ChangeServerPasswordBehavior
        {
           
            private static void Postfix(ref Boolean __result) // Set after awake function
            {
                if (Conf.Server != null)
                {
                    if (Conf.Server.DisableServerPassword)
                    {
                        __result = true;
                    }
                }
            }
        }
       
        // ##################################################### SECTION = MAP

        [HarmonyPatch(typeof(Minimap))]
        public class hookExplore
        {
            [HarmonyReversePatch]
            [HarmonyPatch(typeof(Minimap), "Explore", new Type[] { typeof(Vector3), typeof(float) }) ]
            public static void call_Explore(object instance, Vector3 p, float radius) => throw new NotImplementedException();
        }
        [HarmonyPatch(typeof(ZNet))]
        public class hookZNet
        {
            [HarmonyReversePatch]
            [HarmonyPatch(typeof(ZNet), "GetOtherPublicPlayers", new Type[] { typeof(List<ZNet.PlayerInfo>) })]
            public static void GetOtherPublicPlayers(object instance, List<ZNet.PlayerInfo> playerList) => throw new NotImplementedException();

        }

        [HarmonyPatch(typeof(Minimap), "UpdateExplore")]
        public static class ChangeMapBehavior
        {
            
            private static void Prefix(ref float dt, ref Player player,ref Minimap __instance, ref float ___m_exploreTimer, ref float ___m_exploreInterval, ref List<ZNet.PlayerInfo> ___m_tempPlayerInfo) // Set after awake function
            {
                if (Conf.Map == null) return;

                if (Conf.Map.ShareMapProgression)
                {
                    float explorerTime = ___m_exploreTimer;
                    explorerTime += Time.deltaTime;
                    if (explorerTime > ___m_exploreInterval)
                    {
                        ___m_tempPlayerInfo.Clear();
                        hookZNet.GetOtherPublicPlayers(ZNet.instance, ___m_tempPlayerInfo); // inconsistent returns but works

                        if (___m_tempPlayerInfo.Count() > 0)
                        {
                            foreach (ZNet.PlayerInfo m_Player in ___m_tempPlayerInfo)
                            {
                                hookExplore.call_Explore(__instance, m_Player.m_position, Conf.Map.ExploreRadius);
                            }
                        }
                        
                    }
                }
                // Always reveal for your own, we do this non the less to apply the potentially bigger exploreRadius
                hookExplore.call_Explore(__instance, player.transform.position, Conf.Map.ExploreRadius);
            }
        }
        


        // ##################################################### SECTION = HOTKEYS
        [HarmonyPatch(typeof(Player))]
        public class hookDodgeRoll
        {
            [HarmonyReversePatch]
            [HarmonyPatch(typeof(Player), "Dodge", new Type[] { typeof(Vector3) })]
            public static void Dodge(object instance, Vector3 dodgeDir) => throw new NotImplementedException();
        }
        [HarmonyPatch(typeof(Player), "Update")]
        public static class ApplyHotkeys
        {
            private static void Postfix(ref Player __instance, ref Vector3 ___m_moveDir, ref Vector3 ___m_lookDir, ref GameObject ___m_placementGhost)
            {
                if (Conf.Hotkeys == null) return;

                KeyCode rollKeyForward = Conf.Hotkeys.RollForwards;
                KeyCode rollKeyBackwards = Conf.Hotkeys.RollBackwards;

                if (Input.GetKeyDown(rollKeyBackwards))
                {
                    if(isDebug)
                        Debug.Log("ROLL BACKWARDS");

                    Vector3 dodgeDir = ___m_moveDir;
                    if (dodgeDir.magnitude < 0.1f)
                    {
                        dodgeDir = -___m_lookDir;
                        dodgeDir.y = 0f;
                        dodgeDir.Normalize();
                    }
                    hookDodgeRoll.Dodge(__instance, dodgeDir);
                }
                if (Input.GetKeyDown(rollKeyForward))
                {
                    if(isDebug)
                        Debug.Log("ROLL FORWARDS");

                    Vector3 dodgeDir = ___m_moveDir;
                    if (dodgeDir.magnitude < 0.1f)
                    {
                        dodgeDir = ___m_lookDir;
                        dodgeDir.y = 0f;
                        dodgeDir.Normalize();
                    }
                    hookDodgeRoll.Dodge(__instance, dodgeDir);
                }
            }
        }

        // Helper Functions
        private static float toFloat(string value)
        {
            return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
        }

        public enum ItemType
        {
            None,
            Material,
            Consumable,
            OneHandedWeapon,
            Bow,
            Shield,
            Helmet,
            Chest,
            Ammo = 9,
            Customization,
            Legs,
            Hands,
            Trophie,
            TwoHandedWeapon,
            Torch,
            Misc,
            Shoulder,
            Utility,
            Tool,
            Attach_Atgeir
        }

        public enum PlacementStatus
        {
            // Token: 0x04000FC5 RID: 4037
            Valid = 0,
            // Token: 0x04000FC6 RID: 4038
            Invalid = 1,
            // Token: 0x04000FC7 RID: 4039
            BlockedbyPlayer = 2,
            // Token: 0x04000FC8 RID: 4040
            NoBuildZone = 3,
            // Token: 0x04000FC9 RID: 4041
            PrivateZone = 4,
            // Token: 0x04000FCA RID: 4042
            MoreSpace = 5,
            // Token: 0x04000FCB RID: 4043
            NoTeleportArea = 6,
            // Token: 0x04000FCC RID: 4044
            ExtensionMissingStation = 7,
            // Token: 0x04000FCD RID: 4045
            WrongBiome = 8,
            // Token: 0x04000FCE RID: 4046
            NeedCultivated = 9,
            // Token: 0x04000FCF RID: 4047
            NotInDungeon = 10
        }
    }
}

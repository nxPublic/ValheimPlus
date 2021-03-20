﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using ValheimPlus.Configurations;
using ValheimPlus.RPC;
using ValheimPlus.Utility;

namespace ValheimPlus.GameClasses
{
    /// <summary>
    /// Alter player stamina values
    /// </summary>
    [HarmonyPatch(typeof(Player), "Awake")]
    public static class Player_Awake_Patch
    {
        private static void Postfix(ref Player __instance)
        {
            if (Configuration.Current.Stamina.IsEnabled)
            {
                __instance.m_dodgeStaminaUsage = Helper.applyModifierValue(__instance.m_dodgeStaminaUsage, Configuration.Current.Stamina.dodgeStaminaUsage);
                __instance.m_encumberedStaminaDrain = Helper.applyModifierValue(__instance.m_encumberedStaminaDrain, Configuration.Current.Stamina.encumberedStaminaDrain);
                __instance.m_sneakStaminaDrain = Helper.applyModifierValue(__instance.m_sneakStaminaDrain, Configuration.Current.Stamina.sneakStaminaDrain);
                __instance.m_runStaminaDrain = Helper.applyModifierValue(__instance.m_runStaminaDrain, Configuration.Current.Stamina.runStaminaDrain);
                __instance.m_staminaRegenDelay = Helper.applyModifierValue(__instance.m_staminaRegenDelay, Configuration.Current.Stamina.staminaRegenDelay);
                __instance.m_staminaRegen = Helper.applyModifierValue(__instance.m_staminaRegen, Configuration.Current.Stamina.staminaRegen);
                __instance.m_swimStaminaDrainMinSkill = Helper.applyModifierValue(__instance.m_swimStaminaDrainMinSkill, Configuration.Current.Stamina.swimStaminaDrain);
                __instance.m_jumpStaminaUsage = Helper.applyModifierValue(__instance.m_jumpStaminaUsage, Configuration.Current.Stamina.jumpStaminaDrain);
            }
            if (Configuration.Current.Player.IsEnabled)
            {
                __instance.m_autoPickupRange = Configuration.Current.Player.baseAutoPickUpRange;
                __instance.m_baseCameraShake = Configuration.Current.Player.disableCameraShake ? 0f : 4f;
            }
            if (Configuration.Current.Building.IsEnabled)
            {
                __instance.m_maxPlaceDistance = Configuration.Current.Building.maximumPlacementDistance;
            }
        }
    }

    /// <summary>
    /// Hooks for ABM and AEM
    /// </summary>
    [HarmonyPatch(typeof(Player), "Update")]
    public static class Player_Update_Patch
    {
        private static void Postfix(ref Player __instance, ref Vector3 ___m_moveDir, ref Vector3 ___m_lookDir, ref GameObject ___m_placementGhost, Transform ___m_eye)
        {
            if (!__instance.m_nview.IsValid() || !__instance.m_nview.IsOwner()) return;

            if (Configuration.Current.AdvancedEditingMode.IsEnabled)
            {
                AEM.PlayerInstance = __instance;
                AEM.run();
            }

            if (Configuration.Current.AdvancedBuildingMode.IsEnabled)
            {
                ABM.Run(ref __instance);
            }

            if (Configuration.Current.Hotkeys.IsEnabled)
            {
                ApplyDodgeHotkeys(ref __instance, ref ___m_moveDir, ref ___m_lookDir);
            }

        }

        private static void ApplyDodgeHotkeys(ref Player __instance, ref Vector3 ___m_moveDir, ref Vector3 ___m_lookDir)
        {
            KeyCode rollKeyForward = Configuration.Current.Hotkeys.rollForwards;
            KeyCode rollKeyBackwards = Configuration.Current.Hotkeys.rollBackwards;

            if (Input.GetKeyDown(rollKeyBackwards))
            {
                Vector3 dodgeDir = ___m_moveDir;
                if (dodgeDir.magnitude < 0.1f)
                {
                    dodgeDir = -___m_lookDir;
                    dodgeDir.y = 0f;
                    dodgeDir.Normalize();
                }

                Player_Dodge_ReversePatch.call_Dodge(__instance, dodgeDir);
            }

            if (Input.GetKeyDown(rollKeyForward))
            {
                Vector3 dodgeDir = ___m_moveDir;
                if (dodgeDir.magnitude < 0.1f)
                {
                    dodgeDir = ___m_lookDir;
                    dodgeDir.y = 0f;
                    dodgeDir.Normalize();
                }

                Player_Dodge_ReversePatch.call_Dodge(__instance, dodgeDir);
            }
        }
    }

    /// <summary>
    /// Hook base Dodge method
    /// </summary>
    [HarmonyPatch(typeof(Player))]
    public class Player_Dodge_ReversePatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Player), "Dodge", new Type[] { typeof(Vector3) })]
        public static void call_Dodge(object instance, Vector3 dodgeDir) => throw new NotImplementedException();
    }

    /// <summary>
    /// Update maximum carry weight based on baseMaximumWeight configurations.
    /// </summary>
    [HarmonyPatch(typeof(Player), "GetMaxCarryWeight")]
    public static class Player_GetMaxCarryWeight_Patch
    {
        private static void Prefix(ref Player __instance)
        {
            if (Configuration.Current.Player.IsEnabled)
            {
                __instance.m_maxCarryWeight = Configuration.Current.Player.baseMaximumWeight;
            }
        }
    }


    /// <summary>
    /// Update maximum carry weight based on baseMegingjordBuff configurations.
    /// </summary>
    [HarmonyPatch(typeof(SEMan), "ModifyMaxCarryWeight")]
    public static class Player_ModifyMaxCarryWeight_Patch
    {
        private static bool Prefix(ref SEMan __instance, ref float baseLimit, ref float limit)
        {
            if (!Configuration.Current.Player.IsEnabled) return true;

            foreach (StatusEffect statusEffect in __instance.m_statusEffects)
            {
                if (statusEffect.m_name.Contains("beltstrength"))
                {
                    limit = baseLimit + Configuration.Current.Player.baseMegingjordBuff;
                    statusEffect.ModifyMaxCarryWeight(baseLimit, ref limit);
                }

            }

            return false;
        }
    }


    /// <summary>
    /// Add ValheimPlus intro to compendium.
    /// </summary>
    [HarmonyPatch(typeof(Player), "OnSpawned")]
    public static class Player_OnSpawned_Patch
    {
        private static void Prefix()
        {
            //Show VPlus tutorial raven if not yet seen by the player's character.
            Tutorial.TutorialText introTutorial = new Tutorial.TutorialText()
            {
                m_label = "ValheimPlus Intro",
                m_name = "vplus",
                m_text = "We hope you enjoy the mod, please support our Patreon so we can continue to provide new updates!",
                m_topic = "Welcome to Valheim+"
            };

            if (!Tutorial.instance.m_texts.Contains(introTutorial))
            {
                Tutorial.instance.m_texts.Add(introTutorial);
            }

            Player.m_localPlayer.ShowTutorial("vplus");

            //Only sync on first spawn
            if (VPlusMapSync.ShouldSyncOnSpawn && Configuration.Current.Map.IsEnabled && Configuration.Current.Map.shareMapProgression)
            {
                //Send map data to the server
                VPlusMapSync.SendMapToServer();
                VPlusMapSync.ShouldSyncOnSpawn = false;
            }
        }
    }

    // ToDo have item tooltips be affected.
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateFood))]
    public static class Player_UpdateFood_Transpiler
    {
        private static FieldInfo field_Player_m_foodUpdateTimer = AccessTools.Field(typeof(Player), nameof(Player.m_foodUpdateTimer));
        private static MethodInfo method_ComputeModifiedDt = AccessTools.Method(typeof(Player_UpdateFood_Transpiler), nameof(Player_UpdateFood_Transpiler.ComputeModifiedDT));

        /// <summary>
        /// Replaces the first load of dt inside Player::UpdateFood with a modified dt that is scaled
        /// by the food duration scaling multiplier. This ensures the food lasts longer while maintaining
        /// the same rate of regeneration.
        /// </summary>
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!Configuration.Current.Food.IsEnabled) return instructions;

            List<CodeInstruction> il = instructions.ToList();

            for (int i = 0; i < il.Count - 2; ++i)
            {
                if (il[i].LoadsField(field_Player_m_foodUpdateTimer) &&
                    il[i + 1].opcode == OpCodes.Ldarg_1 /* dt */ &&
                    il[i + 2].opcode == OpCodes.Add)
                {
                    // We insert after Ldarg_1 (push dt) a call to our function, which computes the modified DT and returns it.
                    il.Insert(i + 2, new CodeInstruction(OpCodes.Call, method_ComputeModifiedDt));
                }
            }

            return il.AsEnumerable();
        }

        private static float ComputeModifiedDT(float dt)
        {
            return dt / Helper.applyModifierValue(1.0f, Configuration.Current.Food.foodDurationMultiplier);
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
    public static class Player_GetTotalFoodValue_Transpiler
    {
        private static FieldInfo field_Food_m_health = AccessTools.Field(typeof(Player.Food), nameof(Player.Food.m_health));
        private static FieldInfo field_Food_m_stamina = AccessTools.Field(typeof(Player.Food), nameof(Player.Food.m_stamina));
        private static FieldInfo field_Food_m_item = AccessTools.Field(typeof(Player.Food), nameof(Player.Food.m_item));
        private static FieldInfo field_ItemData_m_shared = AccessTools.Field(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.m_shared));
        private static FieldInfo field_SharedData_m_food = AccessTools.Field(typeof(ItemDrop.ItemData.SharedData), nameof(ItemDrop.ItemData.SharedData.m_food));
        private static FieldInfo field_SharedData_m_foodStamina = AccessTools.Field(typeof(ItemDrop.ItemData.SharedData), nameof(ItemDrop.ItemData.SharedData.m_foodStamina));

        /// <summary>
        /// Replaces loads to the current health/stamina for food with loads to the original health/stamina for food
        /// inside Player::GetTotalFoodValue. This disables food degradation.
        /// </summary>
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!Configuration.Current.Food.IsEnabled) return instructions;

            List<CodeInstruction> il = instructions.ToList();

            if (Configuration.Current.Food.disableFoodDegradation)
            {
                for (int i = 0; i < il.Count; ++i)
                {
                    bool loads_health = il[i].LoadsField(field_Food_m_health);
                    bool loads_stamina = il[i].LoadsField(field_Food_m_stamina);

                    if (loads_health || loads_stamina)
                    {
                        il[i].operand = field_Food_m_item;
                        il.Insert(++i, new CodeInstruction(OpCodes.Ldfld, field_ItemData_m_shared));
                        il.Insert(++i, new CodeInstruction(OpCodes.Ldfld, loads_health ? field_SharedData_m_food : field_SharedData_m_foodStamina));
                    }
                }
            }

            return il.AsEnumerable();
        }
    }

    /// <summary>
    /// Alters stamina of tools, bows and blocking
    /// </summary>
    [HarmonyPatch(typeof(Player), "UseStamina")]
    public static class Player_UseStamina_Patch
    {
        private static void Prefix(ref Player __instance, ref float v)
        {
            if (Configuration.Current.StaminaUsage.IsEnabled)
            {
                string methodName = new StackTrace().GetFrame(2).GetMethod().Name;
                if (methodName.Contains(nameof(Player.UpdatePlacement)) || methodName.Contains(nameof(Player.Repair)) || methodName.Contains(nameof(Player.RemovePiece)))
                {
                    string itemName = __instance.GetRightItem()?.m_shared.m_name;
                    if (itemName == "$item_hammer")
                    {
                        v = Helper.applyModifierValue(v, Configuration.Current.StaminaUsage.hammer);
                    }
                    else if (itemName == "$item_hoe")
                    {
                        v = Helper.applyModifierValue(v, Configuration.Current.StaminaUsage.hoe);
                    }
                    else if (itemName == "$item_cultivator")
                    {
                        v = Helper.applyModifierValue(v, Configuration.Current.StaminaUsage.cultivator);
                    }
                }
                else if (methodName.Equals(nameof(Player.PlayerAttackInput)))
                {
                    ItemDrop.ItemData item = __instance.GetCurrentWeapon();
                    if (item?.m_shared.m_skillType == Skills.SkillType.Bows)
                    {
                        v = Helper.applyModifierValue(v, Configuration.Current.StaminaUsage.bows);
                    }
                }
                else if (methodName.Equals(nameof(Player.BlockAttack)))
                {
                    v = Helper.applyModifierValue(v, Configuration.Current.StaminaUsage.blocking);
                }
            }
        }
    }

	/// <summary>
    /// Starts ABM if not already started
    /// And checks if the player is trying to place a plant/crop too close to another plant/crop
    /// </summary>
    [HarmonyPatch(typeof(Player), "UpdatePlacementGhost")]
    public static class Player_UpdatePlacementGhost_Patch
    {
        private static bool Prefix(ref Player __instance, bool flashGuardStone)
        {
            if (Configuration.Current.AdvancedBuildingMode.IsEnabled)
            {
                ABM.Run(ref __instance);
            }

            if (ABM.isActive)
                return false;

            return true;
        }


        private static void Postfix(ref Player __instance)
        {
            if (ABM.exitOnNextIteration)
            {
                try
                {
                    if (__instance.m_placementMarkerInstance)
                    {
                        __instance.m_placementMarkerInstance.SetActive(false);
                    }
                }
                catch
                {
                }
            }

            if (Configuration.Current.GridAlignment.IsEnabled)
            {
                if (GridAlignment.AlignPressed ^ GridAlignment.AlignToggled)
                    GridAlignment.UpdatePlacementGhost(__instance);
            }

            if (Configuration.Current.Building.noInvalidPlacementRestriction)
            {
                try
                {
                    if (__instance.m_placementStatus == Player.PlacementStatus.Invalid)
                    {
                        __instance.m_placementStatus = Player.PlacementStatus.Valid;
                        __instance.m_placementGhost.GetComponent<Piece>().SetInvalidPlacementHeightlight(false);
                    }
                }
                catch
                {
                }
            }


			if (Configuration.Current.Player.IsEnabled && Configuration.Current.Player.cropNotifier)
            {
                if (__instance.m_placementGhost == null)
                    return;

                // Check to see if the current placement ghost is has a plant component
                Plant plantComponent = __instance.m_placementGhost.GetComponent<Plant>();
                if (plantComponent != null && __instance.m_placementStatus == Player.PlacementStatus.Valid)
                {
                    LayerMask mask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "piece_nonsolid");
                    // Create an array of objects that are within the grow radius of the current placement ghost plant/crop
                    Collider[] array = Physics.OverlapSphere(__instance.m_placementGhost.transform.position, plantComponent.m_growRadius, mask);
                    for (int i = 0; i < array.Length; i++)
                    {
                        // Check if the any of the objects within the array have a plant component
                        // and set the placement status to Need More Space
                        Plant component = array[i].GetComponent<Plant>();
                        if (component != null)
                        {
                            __instance.m_placementStatus = Player.PlacementStatus.MoreSpace;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch]
    public static class AreaRepair
    {
        private static int m_repair_count;

        [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlacement))]
        public static class Player_UpdatePlacement_Transpiler
        {
            private static MethodInfo method_Player_Repair = AccessTools.Method(typeof(Player), nameof(Player.Repair));
            private static AccessTools.FieldRef<Player, Piece> field_Player_m_hoveringPiece = AccessTools.FieldRefAccess<Player, Piece>(nameof(Player.m_hoveringPiece));
            private static MethodInfo method_RepairNearby = AccessTools.Method(typeof(Player_UpdatePlacement_Transpiler), nameof(Player_UpdatePlacement_Transpiler.RepairNearby));

            /// <summary>
            /// Patches the call to Repair from Player::UpdatePlacement with our own stub which handles repairing multiple pieces rather than just one.
            /// </summary>
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Configuration.Current.Building.IsEnabled) return instructions;

                List<CodeInstruction> il = instructions.ToList();

                if (Configuration.Current.Building.enableAreaRepair)
                {
                    // Replace call to Player::Repair with our own stub.
                    // Our stub calls the original repair multiple times, one for each nearby piece.
                    for (int i = 0; i < il.Count; ++i)
                    {
                        if (il[i].Calls(method_Player_Repair))
                        {
                            il[i].operand = method_RepairNearby;
                        }
                    }
                }

                return il.AsEnumerable();
            }

            public static void RepairNearby(Player instance, ItemDrop.ItemData toolItem, Piece _1)
            {
                Piece selected_piece = instance.GetHoveringPiece();
                Vector3 position = selected_piece != null ? selected_piece.transform.position : instance.transform.position;

                List<Piece> pieces = new List<Piece>();
                Piece.GetAllPiecesInRadius(position, Configuration.Current.Building.areaRepairRadius, pieces);

                m_repair_count = 0;

                Piece original_piece = instance.m_hoveringPiece;

                foreach (Piece piece in pieces)
                {
                    bool has_stamina = instance.HaveStamina(toolItem.m_shared.m_attack.m_attackStamina);
                    bool uses_durability = toolItem.m_shared.m_useDurability;
                    bool has_durability = toolItem.m_durability > 0.0f;

                    if (!has_stamina || (uses_durability && !has_durability)) break;

                    // The repair function takes a piece to repair but then completely ignores it and repairs the hovering piece instead...
                    // I really don't like this, but Valheim's spaghetti code makes it required.
                    instance.m_hoveringPiece = piece;
                    instance.Repair(toolItem, _1);
                    instance.m_hoveringPiece = original_piece;
                }

                instance.Message(MessageHud.MessageType.TopLeft, string.Format("{0} pieces repaired", m_repair_count));
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Repair))]
        public static class Player_Repair_Transpiler
        {
            private static MethodInfo method_Character_Message = AccessTools.Method(typeof(Character), nameof(Character.Message));
            private static MethodInfo method_MessageNoop = AccessTools.Method(typeof(Player_Repair_Transpiler), nameof(Player_Repair_Transpiler.MessageNoop));

            /// <summary>
            /// Noops the original message notification when one piece is repaired, and counts them instead - the other transpiler
            /// will dispatch one notification for a batch of repairs using this count.
            /// </summary>
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Configuration.Current.Building.IsEnabled) return instructions;

                List<CodeInstruction> il = instructions.ToList();

                if (Configuration.Current.Building.enableAreaRepair)
                {
                    // Replace calls to Character::Message with our own noop stub
                    // We don't want to spam messages for each piece so we patch the messages out here and dispatch our own messages in the other transpiler.
                    // First call pushes 1, then subsequent calls 0 - the first call is the branch where the repair succeeded.
                    int count = 0;
                    for (int i = 0; i < il.Count; ++i)
                    {
                        if (il[i].Calls(method_Character_Message))
                        {
                            il[i].operand = method_MessageNoop;
                            il.Insert(i++, new CodeInstruction(count++ == 0 ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0, null));
                        }
                    }
                }

                return il.AsEnumerable();
            }

            public static void MessageNoop(Character _0, MessageHud.MessageType _1, string _2, int _3, Sprite _4, int repaired)
            {
                m_repair_count += repaired;
            }
        }
    }

    [HarmonyPatch(typeof(Player), "Update")]
    public static class GridAlignment
    {
        public static int DefaultAlignment = 100;
        public static bool AlignPressed = false;
        public static bool AlignToggled = false;

        private static void Postfix(ref Player __instance)
        {
            if (!Configuration.Current.GridAlignment.IsEnabled)
                return;

            if (Input.GetKeyDown(Configuration.Current.GridAlignment.align))
                AlignPressed = true;
            if (Input.GetKeyUp(Configuration.Current.GridAlignment.align))
                AlignPressed = false;

            if (Input.GetKeyDown(Configuration.Current.GridAlignment.changeDefaultAlignment))
            {
                if (DefaultAlignment == 50)
                    DefaultAlignment = 100;
                else if (DefaultAlignment == 100)
                    DefaultAlignment = 200;
                else if (DefaultAlignment == 200)
                    DefaultAlignment = 400;
                else
                    DefaultAlignment = 50;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Default grid alignment set to " + (DefaultAlignment / 100f));
            }

            if (Input.GetKeyDown(Configuration.Current.GridAlignment.alignToggle))
            {
                AlignToggled ^= true;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Grid alignment by default " + (AlignToggled ? "enabled" : "disabled"));
            }
        }

        static float FixAlignment(float f)
        {
            int i = (int)Mathf.Round(f * 100f);
            if (i <= 0)
                return DefaultAlignment / 100f;
            if (i <= 50)
                return 0.5f;
            if (i <= 100)
                return 1f;
            if (i <= 200)
                return 2f;
            return 4f;
        }

        public static void GetAlignment(Piece piece, out Vector3 alignment, out Vector3 offset)
        {
            var points = new System.Collections.Generic.List<Transform>();
            piece.GetSnapPoints(points);
            if (points.Count != 0)
            {
                Vector3 min = Vector3.positiveInfinity;
                Vector3 max = Vector3.negativeInfinity;
                foreach (var point in points)
                {
                    var pos = point.localPosition;
                    min = Vector3.Min(min, pos);
                    max = Vector3.Max(max, pos);
                }
                alignment = max - min;
                alignment.x = FixAlignment(alignment.x);
                alignment.y = FixAlignment(alignment.y);
                alignment.z = FixAlignment(alignment.z);
                // Align at top
                offset = max;
                if (piece.name == "iron_grate" || piece.name == "wood_gate")
                {
                    // Align at bottom, not top
                    offset.y = min.y;
                }
                if (piece.name == "wood_gate")
                {
                    alignment.x = 4;
                }
            }
            else
            {
                if (piece.m_notOnFloor || piece.name == "sign" || piece.name == "itemstand")
                {
                    alignment = new Vector3(0.5f, 0.5f, 0);
                    offset = new Vector3(0, 0, 0);
                    if (piece.name == "sign")
                        alignment.y = 0.25f;
                }
                else if (piece.name == "piece_walltorch")
                {
                    alignment = new Vector3(0, 0.5f, 0.5f);
                    offset = new Vector3(0, 0, 0);
                }
                else
                {
                    alignment = new Vector3(0.5f, 0, 0.5f);
                    offset = new Vector3(0, 0, 0);
                }
            }
        }

        public static float Align(float value, out float alpha)
        {
            float result = Mathf.Round(value);
            alpha = value - result;
            return result;
        }


        public static void UpdatePlacementGhost(Player player)
        {
            if (player.m_placementGhost == null)
                return;

            if (ABM.isActive)
                return;

            bool altMode = ZInput.GetButton("AltPlace") || ZInput.GetButton("JoyAltPlace");

            var piece = player.m_placementGhost.GetComponent<Piece>();

            var newVal = piece.transform.position;
            newVal = Quaternion.Inverse(piece.transform.rotation) * newVal;

            Vector3 alignment;
            Vector3 offset;
            GetAlignment(piece, out alignment, out offset);
            newVal += offset;
            var copy = newVal;
            newVal = new Vector3(newVal.x / alignment.x, newVal.y / alignment.y, newVal.z / alignment.z);
            float alphaX, alphaY, alphaZ;
            newVal = new UnityEngine.Vector3(Align(newVal.x, out alphaX), Align(newVal.y, out alphaY), Align(newVal.z, out alphaZ));
            if (altMode)
            {
                float alphaMin = 0.2f;
                if (Mathf.Abs(alphaX) >= alphaMin && Mathf.Abs(alphaX) >= Mathf.Abs(alphaY) && Mathf.Abs(alphaX) >= Mathf.Abs(alphaZ))
                    newVal.x += Mathf.Sign(alphaX);
                else if (Mathf.Abs(alphaY) >= alphaMin && Mathf.Abs(alphaY) >= Mathf.Abs(alphaZ))
                    newVal.y += Mathf.Sign(alphaY);
                else if (Mathf.Abs(alphaZ) >= alphaMin)
                    newVal.z += Mathf.Sign(alphaZ);
            }
            newVal = new Vector3(newVal.x * alignment.x, newVal.y * alignment.y, newVal.z * alignment.z);
            if (alignment.x <= 0)
                newVal.x = copy.x;
            if (alignment.y <= 0)
                newVal.y = copy.y;
            if (alignment.z <= 0)
                newVal.z = copy.z;
            newVal -= offset;

            newVal = piece.transform.rotation * newVal;
            piece.transform.position = newVal;
        }
    }
    
    /// <summary>
    /// Configures guardian buff duration and cooldown
    /// </summary>
    [HarmonyPatch(typeof(Player), "SetGuardianPower")]
    public static class Player_SetGuardianPower_Patch
    {
        private static void Postfix(ref Player __instance)
        {
            if (Configuration.Current.Player.IsEnabled)
            {
                if(__instance.m_guardianSE) 
                {
                    __instance.m_guardianSE.m_ttl = Configuration.Current.Player.guardianBuffDuration;
                    __instance.m_guardianSE.m_cooldown = Configuration.Current.Player.guardianBuffCooldown;
                }
            }
        }
    }
    
    /// <summary>
    /// Skip Intro
    /// </summary>
    [HarmonyPatch(typeof (Player), "OnSpawned")]
    public class OnSpawned_Patch
    {
        [HarmonyPrefix]
        internal static void Prefix(Player __instance)
        {
            if (Configuration.Current.Player.skipintro)
            {
                __instance.m_firstSpawn = false;
            }
        }
    }
    
    /// <summary>
    /// PVP Switch Section
    /// </summary>
    [HarmonyPatch(typeof(Player), "SetLocalPlayer")]
    public static class SetLocalPlayer_Patch
    {
        public static void Postfix()
        {
            if (Configuration.Current.Player.pvpveactv)
            {
                if (Configuration.Current.Player.pvpveswitch)
                    ZLog.LogWarning("[PVPVE] PVP MODE!");
                else if (Configuration.Current.Player.pvpveswitch == false)
                    ZLog.LogWarning("[PVPVE] PVE MODE!");
                
                Player.m_localPlayer.SetPVP(Configuration.Current.Player.pvpveswitch);
            }
        }
    }

    [HarmonyPatch(typeof(Player), "CanSwitchPVP")]
    class CanSwitchPVP_Patch
    {
        static bool Prefix(ref bool __result)
        {
            if (Configuration.Current.Player.pvpveactv)
            {
                __result = !Configuration.Current.Player.pvpveactv;
                return false;
            }
            return true;
        }
    }
    
    [HarmonyPatch(typeof(Player), "SetPVP")]
    class SetPVP_Patch
    {
        public static bool Prefix(Player __instance)
        {
            if (Configuration.Current.Player.pvpveactv)
            {
                __instance.m_pvp = Configuration.Current.Player.pvpveswitch;
                if (Configuration.Current.Player.pvpveswitch)
                    __instance.m_nview.GetZDO().Set("pvp", __instance.m_pvp);
                else if (Configuration.Current.Player.pvpveswitch == false)
                    __instance.m_nview.GetZDO().Set("pve", __instance.m_pvp);
                return false;
            }
            return true;
        }
    }
}

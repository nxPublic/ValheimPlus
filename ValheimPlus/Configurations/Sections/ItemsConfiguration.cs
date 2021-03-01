﻿namespace ValheimPlus.Configurations.Sections
{
    public class ItemsConfiguration : ServerSyncConfig<ItemsConfiguration>
    {
        public bool noTeleportPrevention { get; set; } = false;
        public float baseItemWeightReduction { get; set; } = 0;
        public float itemStackMultiplier { get; internal set; } = 1;
        public int increasedPickableMultiplier { get; internal set; } = 1;
        public int increasedLootMultiplier { get; internal set; } = 1;
    }

}

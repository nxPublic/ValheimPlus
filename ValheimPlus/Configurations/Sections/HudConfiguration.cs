﻿namespace ValheimPlus.Configurations.Sections
{
    public class HudConfiguration : BaseConfig<HudConfiguration>
    {
        public bool showRequiredItems { get; internal set; } = false;
        public bool experienceGainedNotifications { get; internal set; } = false;
        public float chatMessageDistance { get; internal set; }
        public bool removeDamageFlash { get; internal set; } = false;
    }
}

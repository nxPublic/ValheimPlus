﻿// ValheimPlus

namespace ValheimPlus.Configurations.Sections
{
    [ConfigurationSection("Ward settings")]
    public class WardConfiguration : ServerSyncConfig<WardConfiguration>
    {
        [Configuration("The range of wards in meters", ActivationTime.AfterRestart)]
        public float wardRange { get; internal set; } = 20;
    }
}
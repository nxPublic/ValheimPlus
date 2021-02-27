﻿// ValheimPlus

namespace ValheimPlus.Configurations.Sections
{
    public class BuildingConfiguration : ServerSyncConfig<BuildingConfiguration>
    {
        [Configuration("Remove some of the Invalid placement messages, most notably provides the ability to place objects into other objects", ActivationTime.Immediately)]
        public bool noInvalidPlacementRestriction { get; set; } = false;

        [Configuration("Removes the weather damage from rain", ActivationTime.Immediately)]
        public bool noWeatherDamage { get; set; } = false;

        [Configuration("The maximum range that you can place build objects at", ActivationTime.Immediately)]
        public float maximumPlacementDistance { get; set; } = 5;
    }
}
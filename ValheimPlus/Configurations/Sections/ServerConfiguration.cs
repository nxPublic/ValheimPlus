﻿// ValheimPlus

namespace ValheimPlus.Configurations.Sections
{
    [ConfigurationSection("Server settings")]
    public class ServerConfiguration : ServerSyncConfig<ServerConfiguration>
    {
        [Configuration("Modify the amount of players on your Server", ActivationTime.AfterRestartServer)]
        public int maxPlayers { get; set; } = 10;

        [Configuration("Removes the requirement to have a server password", ActivationTime.AfterRestartServer)]
        public bool disableServerPassword { get; set; } = false;

        [Configuration(
            "This settings add a version control check to make sure that people that try to join your game or the server you try to join has V+ installed", ActivationTime.AfterRestartServer)]
        public bool enforceMod { get; set; } = true;

        [Configuration("The total amount of data that the server and client can send per second in kilobyte", ActivationTime.Immediately)]
        public int dataRate { get; set; } = 60; // 614440 == 60kbs

        [Configuration("The interval in seconds that the game auto saves at (client only)", ActivationTime.Immediately)]
        public float autoSaveInterval { get; set; } = 1200;
    }
}
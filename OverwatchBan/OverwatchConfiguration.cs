using System;
using Rocket.API;

namespace Alexr03.Overwatch
{
    public class OverwatchConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public bool ShowDebugInfo;
        public int DatabasePort;

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "banlist";
            ShowDebugInfo = false;
            DatabasePort = 3306;
        }
    }
}
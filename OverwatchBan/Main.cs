using Rocket.API.Collections;
using Rocket.Core.Logging;
using Steamworks;
using System.Collections.Generic;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using Rocket.Unturned.Permissions;
using Rocket.Unturned;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using System.IO;

namespace Alexr03.Overwatch
{
    public class Overwatch : RocketPlugin<OverwatchConfiguration>
    {
        public static Overwatch Instance;

        private DateTime timeDelay = DateTime.Now;

        public DatabaseManager Database;

        public static Dictionary<CSteamID, string> Players = new Dictionary<CSteamID, string>();

        protected override void Load()
        {
            Instance = this;
            Rocket.Core.Logging.Logger.Log("Overwatch has loaded!");
            Database = new DatabaseManager();
            UnturnedPermissions.OnJoinRequested += Events_OnJoinRequested;
            U.Events.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
        }

        public Overwatch()
        {
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= RocketServerEvents_OnPlayerConnected;
        }

        private void FixedUpdate()
        {

        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    {"default_banmessage","You have been banned by a Overwatch"},
                    {"command_generic_invalid_parameter","Invalid parameter"},
                    {"command_generic_player_not_found","Player not found"},
                    {"command_ban_public_reason", "The player {0} was banned by Overwatch"},
                    {"command_ban_public","The player {0} was banned by overwatch"},
                    {"command_ban_private_default_reason","you were banned from the server via Overwatch"},
                    {"command_kick_public_reason", "The player {0} was kicked by Overwatch"},
                    {"command_kick_public","The player {0} was kicked by Overwatch"},
                    {"general_not_found","Player not found."},
                    {"general_invalid_parameter", "Invalid parameter."},
                    {"report_pending", "There is already a report pending, please wait."},
                    {"report", "Thank you for your report."},
                };
            }
        }

        public static KeyValuePair<CSteamID, string> GetPlayer(string search)
        {
            KeyValuePair<CSteamID, string> keyValuePair;
            foreach (KeyValuePair<CSteamID, string> pair in Overwatch.Players)
            {
                if ((pair.Key.ToString().ToLower().Contains(search.ToLower()) ? true : pair.Value.ToLower().Contains(search.ToLower())))
                {
                    keyValuePair = pair;
                    return keyValuePair;
                }
            }
            keyValuePair = new KeyValuePair<CSteamID, string>(new CSteamID((ulong)0), null);
            return keyValuePair;
        }

        void RocketServerEvents_OnPlayerConnected(UnturnedPlayer player)
        {
            if (!Players.ContainsKey(player.CSteamID))
                Players.Add(player.CSteamID, player.CharacterName);
        }

        public void Events_OnJoinRequested(CSteamID player, ref ESteamRejection? rejection)
        {
            try
            {
                string banned = Database.IsBanned(player.ToString());
                if (banned != null)
                {
                    if (banned == "") banned = Translate("default_banmessage");
                    rejection = ESteamRejection.AUTH_PUB_BAN;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
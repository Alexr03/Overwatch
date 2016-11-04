using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;
using System;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace Alexr03.Overwatch
{
    public class CommandOKick : IRocketCommand
    {
        public string Help
        {
            get { return "Kicks a player monitored by overwatch"; }
        }

        public string Name
        {
            get { return "okick"; }
        }

        public string Syntax
        {
            get { return "<player> [reason]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "overwatch.okick" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {

            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(caller, Overwatch.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }
            UnturnedPlayer playerToKick = UnturnedPlayer.FromName(command[0]);
            if (playerToKick == null)
            {
                UnturnedChat.Say(caller, Overwatch.Instance.Translate("command_generic_player_not_found"));
                return;
            }
            if (command.Length >= 2)
            {
                UnturnedChat.Say(Overwatch.Instance.Translate("command_kick_public_reason", playerToKick.SteamName, command[1]));
                Provider.kick(playerToKick.CSteamID, "You kicked by a Overwatch Agent");
            }
            else
            {
                UnturnedChat.Say(Overwatch.Instance.Translate("command_kick_public", playerToKick.SteamName));
                Provider.kick(playerToKick.CSteamID, Overwatch.Instance.Translate("command_kick_private_default_reason"));
            }
        }
    }
}
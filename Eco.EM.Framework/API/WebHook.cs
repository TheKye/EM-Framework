using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eco.EM.Framework.Discord;
using Eco.EM.Framework.Plugins;
using Newtonsoft.Json;

namespace Eco.EM.Framework
{
    public class WebHook
    {
        internal static void PostToWebhook()
        {
            var url = BasePlugin.Obj.Config.DiscordWebhookURL;
            if (string.IsNullOrWhiteSpace(url))
                return;
            DiscordWebhook hook = new();
            hook.Url = url;

            DiscordMessage message = new()
            {
                Content = "",
                TTS = false, //read message to everyone on the channel
                Username = "Elixr Mods",
                AvatarUrl = "https://elixrmods.com/Assets/img/logos/EMICON-text.png"
            };

            var messageContent = string.IsNullOrWhiteSpace(EMVersioning.modVersion) ? EMVersioning.needsUpdate : EMVersioning.modVersion;
            //embeds
            DiscordEmbed embed = new()
            {
                Title = "Elixr Mods, Mod Versions",
                Description = messageContent,
                Url = "",
                Timestamp = DateTime.Now,
                Color = Color.Purple, //alpha will be ignored, you can use any RGB color
                Footer = new EmbedFooter() { Text = "Brought to you by Elixr Mods", IconUrl = "https://elixrmods.com/Assets/img/logos/EMICON-text.png" },
            };

            //set embed
            message.Embeds = new List<DiscordEmbed>
            {
                embed
            };

            hook.Send(message);
        }
    }
}

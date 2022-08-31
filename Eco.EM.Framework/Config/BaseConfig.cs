using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework
{
    public class BaseConfig
    {
        [LocDescription("Whether or not to check EM pack versions and display them.")]
        public bool VersionDisplayEnabled { get; set; } = true;

        [LocDescription("Used to Auto Skip Tutorial For all Players (Not Recommended for new player Friendly servers)")]
        public bool AutoSkipTutorial { get; set; } = false;

        [LocDisplayName("Wipe Groups Data file on New World")]
        [LocDescription("Determine if the Groups File should be wiped on each world reset or not (You will lose all groups and assigned commands in those groups)")]
        public bool WipeGroupsFileOnFreshWorld { get; set; } = false;

        [LocDescription("Check for updates will send a query every so often to check if mods that use the versioning system, need an update")]
        public bool CheckForUpdates { get; set; } = false;

        [LocDescription("This is a toggle option to post notifications to discord if mods need an update")]
        public bool PostToDiscord { get; set; } = false;

        [LocDescription("The Webhook URL used to post to discord when mod updates are available")]
        public string DiscordWebhookURL { get; set; } = "";
        
    }
}

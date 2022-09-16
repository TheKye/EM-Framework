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

        [LocDisplayName("Enabel or disable the Web API Extenstions for your server")]
        [LocDescription("By enabling this, members can use your servers connection info to access the Web API methods added by the framework (Which are rate limited) to get information from your server for 3rd party tools like cost calculators etc.")]
        public bool EnableWebAPI { get; set; } = true;

        [LocDescription("Check for updates will send a query every so often to check if mods that use the versioning system, need an update")]
        public bool CheckForUpdates { get; set; } = false;

        [LocDescription("This is a toggle option to post notifications to discord if mods need an update")]
        public bool PostToDiscord { get; set; } = false;

        [LocDescription("The Webhook URL used to post to discord when mod updates are available")]
        public string DiscordWebhookURL { get; set; } = "";
        
    }
}

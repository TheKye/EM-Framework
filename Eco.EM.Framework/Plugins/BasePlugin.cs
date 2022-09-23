using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Core.Utils.Logging;
using Eco.EM.Framework.Logging;
using Eco.EM.Framework.Utils;
using Eco.Gameplay.EcopediaRoot;
using Eco.Gameplay.GameActions;
using Eco.Gameplay.Players;
using Eco.Gameplay.Tutorial;
using Eco.ModKit;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Plugins
{
    [LocDisplayName("EM Base Plugin")]
    public class BasePlugin : Singleton<BasePlugin>, IInitializablePlugin, IModKitPlugin, IConfigurablePlugin, IShutdownablePlugin
    {
        public EMVersioning Versions = new();
        public Timer Timer;

        private readonly PluginConfig<BaseConfig> config;
        public BasePlugin()
        {
            this.config = new PluginConfig<BaseConfig>("EMBase");
        }

        public IPluginConfig PluginConfig => this.config;
        public BaseConfig Config => this.config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => this.config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded and using Version: {Defaults.version}, All Systems Active";

        public void Initialize(TimedTask timer)
        {
            this.SaveConfig();
            if (Config.VersionDisplayEnabled)
                EMVersioning.GetInit();
            if (Config.CheckForUpdates)
                Timer = new(Timer_tick, null, 900000, 900000);

            if (Config.AutoSkipTutorial)
            {
                UserManager.NewUserJoinedEvent.Add(x =>
                {
                    TutorialTasks.SkipAllTutorials(x);
                });
            }
            EcopediaGenerator.GenerateEcopediaPageFromFile("ModDocumentation.xml", "Eco.EM.Framework.Ecopedia", "Elixr Mods");

            ActionUtil.AddListener(new Listeners.GameActionListener());
            EcopediaGenerator.BuildPages();

            //By Default Disable Web API for official Servers
            if (Eco.Plugins.Networking.NetworkManager.Config.Description.Contains("[SLG]"))
                Config.EnableWebAPI = false;
            
        }

        static void Timer_tick(object state)
        {
            if (string.IsNullOrWhiteSpace(Obj.Config.DiscordWebhookURL))
            {
                LoggingUtils.Error("Can't Post Update Information without a webhook url.");
                return;
            }
            LoggingUtils.Write("Checking For Updates");
            EMVersioning.CheckUpdate();
        }
        public override string ToString()
        {
            return Localizer.DoStr("EM Framework");
        }

        public string GetCategory() => "Elixr Mods";

        public Task ShutdownAsync() => EcopediaGenerator.ShutDown();
    }
}

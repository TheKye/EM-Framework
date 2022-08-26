using Eco.Core.Controller;
using Eco.Core.Plugins.Interfaces;
using Eco.Gameplay.Civics.Misc;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems;
using Eco.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eco.Gameplay.Systems.ViewEditor;
using static Eco.Gameplay.Systems.ViewEditorUtils;

namespace Eco.EM.Framework.UI
{
    public partial class ConfigUI
    {
        
        public static void PrepConfigUI(User user, IController values, string modName, ISaveablePlugin plugin, Action<object> action)
        {
            Type type = plugin.GetType();
            PopUI(user, type, Localizer.DoStr($"{modName} Config"), values, plugin: plugin, setVal: action);
        }

        private static void PopUI(User user, Type type, LocString? title, IController existing, Action<object> setVal, ISaveablePlugin plugin = null, WindowType windowType = WindowType.Small, LocString? buttonName = null)
        {
            // if existing == null, create new instance in draft mode
            IController objToEdit = existing != null ? Cloner.Clone(existing) : Activator.CreateInstance(type) as IController;

            ViewEditor.Edit(user, objToEdit,
                windowType: windowType,
                overrideTitle: title,
                onSubmit: val => setVal(objToEdit),
                savePlugin: plugin,
                buttonText: buttonName);
        }
    }
}

using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eco.EM.Framework.UI;
using Eco.EM.Framework.Resolvers;

namespace Eco.EM.Framework.AdminUtils
{
    [ChatCommandHandler]
    public class ProcUI
    {
        [ChatCommand("")]
        public static void ProcNewUI(User user)
        {
            ConfigUI.PrepConfigUI(user, null, "EM Framework", EMConfigurePlugin.Obj, null);
        }
    }
}

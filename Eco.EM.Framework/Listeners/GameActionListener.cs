using Eco.Core.Utils;
using Eco.Gameplay.GameActions;
using Eco.Stats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Listeners
{  
    [NoStats, NoLaws, Category("Hidden")]
    public class GameActionListener : IGameActionAware
    {
        #region Events
        public static event Action<FirstLogin> FirstLoginEvent;
        #endregion Events

        #region Listener
        public void ActionPerformed(GameAction action)
        {
            if (action is null)
            {
                return;
            }

            if (action is FirstLogin)
            {
                if (FirstLoginEvent is not null) FirstLoginEvent.Invoke(action as FirstLogin);
            }
        }
       
        public LazyResult ShouldOverrideAuth(GameAction action)
        {
            return LazyResult.FailedNoMessage;
        }
        #endregion Listener

        #region Subscribers
        public static void SubscribeFirstLogin(Action<FirstLogin> sMethod)
        {
            FirstLoginEvent += sMethod;
        }

        public static void UnsubscribeFirstLogin(Action<FirstLogin> uMethod)
        {
            FirstLoginEvent -= uMethod;
        }
        #endregion Subscribers
    }
}

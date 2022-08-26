using Eco.Shared.Serialization;
using System;

namespace Eco.EM.Framework.Permissions
{
    [Serializable]
    public class CommandGroupsConfig
    {
        [Serialized] public bool DefaultAdminBehaviour { get; set; }
        [Serialized] public bool DefaultUserBehaviour { get; set; }

        public CommandGroupsConfig()
        {
            DefaultAdminBehaviour = true;
            DefaultUserBehaviour = true;
        }
    }
}

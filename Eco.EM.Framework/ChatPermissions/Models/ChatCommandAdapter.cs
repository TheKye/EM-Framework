using Eco.EM.Framework.Groups;
using Eco.Gameplay.Systems.Chat;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Newtonsoft.Json;
using System.Linq;

namespace Eco.EM.Framework.Permissions
{
    public class ChatCommandAdapter : IGroupAuthorizable
    {
        public string Identifier { get; private set; }
        public string ShortCut { get; private set; }
        public bool BlackListed { get; set; }

        [JsonConstructor]
        public ChatCommandAdapter(string ident)
        {
            Identifier = ident;
        }

        public ChatCommandAdapter(ChatCommand c)
        {
            Identifier = c.Name;
            ShortCut = c.ShortCut;
        }

        public bool Permit(SimpleGroupUser user)
        {
            // get all the groups the user is in.
            var groups = GroupsManager.API.AllGroups().Where(grp => grp.GroupUsers.Any(sgu => sgu.Name == user.Name));

            // check if any of those groups have the permission.
            return groups.Any(grp => grp.Permissions.Any(p => p.Identifier == Identifier && p.BlackListed == false));
        }
    }
}

using System;
using System.Collections;

namespace Eco.EM.Framework.Groups
{
    [Serializable]
    public class SimpleGroupUser : IEquatable<SimpleGroupUser>
    {
         public string Name { get; private set; }
         public string SlgID { get; private set; }
         public string SteamID { get; private set; }

        public SimpleGroupUser(string name, string slgid, string steamid)
        {
            this.Name = name;
            this.SlgID = slgid;
            this.SteamID = steamid;
        }

        public bool Equals(SimpleGroupUser other)
        {
            return this.Name == other.Name && this.SlgID == other.SlgID && this.SteamID == other.SteamID;
        }
    }
}

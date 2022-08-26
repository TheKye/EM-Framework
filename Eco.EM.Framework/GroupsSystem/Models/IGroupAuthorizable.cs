using Eco.Shared.Serialization;
using System;

namespace Eco.EM.Framework.Groups
{
    public interface IGroupAuthorizable
    {
        string Identifier { get; }
        bool BlackListed { get; set; }
        bool Permit(SimpleGroupUser user) => BlackListed;
    }
}

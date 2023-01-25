using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Interactions;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.AdminUtils
{
    [Serialized]
    [LocDisplayName("Admin Testing Block")]
    [NotSpawnable]
    public partial class TestingBlockItem : WorldObjectItem<TestingBlockObject>
    {
    }


    [Serialized]
    [RequireComponent(typeof(AdminUiUtil))]
    public partial class TestingBlockObject : WorldObject
    {
        public override InteractResult OnActInteract(InteractionContext context)
        {
            if(context.Player.IsAdmin())
            return base.OnActInteract(context);
            else
            {
                context.Player.ErrorLocStr("Only an Admin May interact with this");
                return InteractResult.Failure(Localizer.DoStr("Only an Admin May interact with this"));
            }
        }

        public override InteractResult OnActLeft(InteractionContext context)
        {
            if (context.Player.IsAdmin())
                return base.OnActInteract(context);
            else
            {
                context.Player.ErrorLocStr("Only an Admin May interact with this");
                return InteractResult.Failure(Localizer.DoStr("Only an Admin May interact with this"));
            }
        }
    }
}

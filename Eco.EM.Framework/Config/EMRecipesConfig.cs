using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMRecipesConfig
    {
        [LocDescription("Recipes loaded by modules. ANY change to this list will require a server restart to take effect.")]
        [LocDisplayName("Recipes")]
        public SerializedSynchronizedCollection<RecipeModel> EMRecipes { get; set; } = new();
    }
}

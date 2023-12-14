using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMConfigureBaseConfig
    {

        [LocDisplayName("Remove Stockpile Stack Restriction")]
        [LocDescription("Removes the Stack Limit Restriction in Stockpiles. Requires Manual Removal after Disable. Requires a Server Restart.")]
        public bool OverrideVanillaStockpiles { get; set; }

        [LocDisplayName("Enable Lucky Strike For All")]
        [LocDescription("Enables Lucky Strike as a Default Thing, which means the talent is no longer needed, Requires Manual Removal after Disable. Requires Server Restart.")]
        public bool EnableGlobalLuckyStrike { get; set; }
    }
}

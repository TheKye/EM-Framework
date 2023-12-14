using Eco.Core.Controller;
using Eco.EM.Framework.Extentsions;
using Eco.EM.Framework.Helpers;
using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Mods.TechTree;
using Eco.Shared.Localization;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using Eco.Shared.Math;
using System;

namespace Eco.EM.Framework.Components
{
    [Eco, AutogenClass, LocDisplayName("Automatic Doors")]
    public class AutoDoorComponent : WorldObjectComponent
    {
        private StatusElement status;

        float detectionRange = 4;
        bool allowGuests = false;

        [Eco, ClientInterfaceProperty, GuestHidden]
        public float DetectionRange
        {
            get => this.detectionRange;
            set
            {
                if (value == this.detectionRange) return;
                this.detectionRange = value;
                this.Changed(nameof(this.DetectionRange));
            }
        }

        [Eco, ClientInterfaceProperty, GuestHidden]
        public bool AllowGuests
        {
            get => this.allowGuests;
            set
            {
                if (value == this.allowGuests) return;
                this.allowGuests = value;
                this.Changed(nameof(this.AllowGuests));
            }
        }

        public override bool Enabled => true;

        [Serialized] public bool enabled { get; set; }

        public WorldObject Object { get; set; }
        public AutoDoorComponent() { }
        public AutoDoorComponent(float detectionRange, bool allowGuests)
        {
            DetectionRange = detectionRange;
            AllowGuests = allowGuests;
        }

        public void Initialize(WorldObject obj)
        {
            Object = obj;
            base.Initialize();
            if (!Object.HasComponent<StatusComponent>())
                throw new NotImplementedException("Status Component Is Required in order to use the Auto Door Component.");
            status = Object.GetComponent<StatusComponent>().CreateStatusElement();
            status.SetStatusMessage(true, Localizer.DoStr(string.Format("Automatic Door Module Installed, Automatic Doors Enabled and are {0}", enabled ? "Working!" : "Idle.")));
        }

        [RPC, Autogen]
        public virtual void EnableAutoDoor(Player player)
        {
            enabled = true;
        }

        [RPC, Autogen]
        public virtual void DisableAutoDoor(Player player)
        {
            enabled = false;
        }

        public override void Tick()
        {
            if (enabled)
                OperateAutoDoor();
            base.Tick();
            status.SetStatusMessage(true, Localizer.DoStr(string.Format("Automatic Door Module Installed, Automatic Doors Enabled and are {0}. Guests are {1}", enabled ? "Working!" : "Idle.", allowGuests ? "Allowed" : "Not Allowed")));
        }

        protected virtual void OperateAutoDoor()
        {
            if (!enabled) return;
            NetObjectManager.Default
                .GetObjectsWithin(Parent.Position, 2)
                .ForEach(obj =>
                {
                    switch (obj)
                    {
                        case DoorObject wo when Parent is WorldObject:
                            if (wo.GetComponent<DoorComponent>().IsOpen && (PlayerSensor.AuthorizedPersonnelNear(wo, (int)DetectionRange) || wo.GetComponent<DoorComponent>().IsOpen && PlayerSensor.AnyoneNear(wo, (int)DetectionRange) && AllowGuests))
                                return;
                            else if (PlayerSensor.AnyoneNear(wo, (int)DetectionRange) && AllowGuests)
                                wo.GetComponent<DoorComponent>().SetOpen(PlayerSensor.UserNear(wo, (int)DetectionRange), PlayerSensor.AnyoneNear(wo, (int)DetectionRange));
                            else
                                wo.GetComponent<DoorComponent>().SetOpen(PlayerSensor.UserNear(wo, (int)DetectionRange), PlayerSensor.AuthorizedPersonnelNear(wo, (int)DetectionRange));
                            break;

                        case EmDoor woe when Parent is WorldObject:
                            if (woe.Open && (PlayerSensor.AuthorizedPersonnelNear(woe, (int)DetectionRange) || woe.Open && PlayerSensor.AnyoneNear(woe, (int)DetectionRange) && AllowGuests))
                                return;

                            else if (PlayerSensor.AnyoneNear(woe, (int)DetectionRange) && AllowGuests)
                                woe.SetOpen(PlayerSensor.AnyoneNear(woe, (int)DetectionRange));
                            else
                                woe.SetOpen(PlayerSensor.AuthorizedPersonnelNear(woe, (int)DetectionRange));
                            break;
                    }
                });
        }
    }
}

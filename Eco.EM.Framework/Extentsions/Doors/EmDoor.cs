using Eco.Gameplay.Audio;
using Eco.Gameplay.Components;
using Eco.Gameplay.Interactions;
using Eco.Gameplay.Objects;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using Eco.Gameplay.Auth;
using Eco.Shared.IoC;
using System;
using Eco.Gameplay.Items;
using Eco.Mods.TechTree;
using Eco.Gameplay.Interactions.Interactors;
using Eco.Gameplay.Players;
using Eco.Shared.SharedTypes;
using Eco.Core.Controller;
using Eco.Gameplay.Systems.EnvVars;

/// <summary>
/// This is out basic component to add to all our doors for their functionality, This is just so 
/// we don't need to add all these on each door
/// </summary>

namespace Eco.EM.Framework.Extentsions
{

    [Serialized]
    [RequireComponent(typeof(StatusComponent), null)]
    public abstract class EmDoor : WorldObject
    {
        [Serialized] public  bool OpensOut { get; set; }
        [Serialized, Notify, EnvVar] public bool Open { get; set; }
        [Serialized] public bool HasModule { get; set; }
        [Serialized] public float Range { get; set; }

        [Interaction(InteractionTrigger.RightClick, "Open", DisallowedEnvVars = new[] { nameof(Open) })]
        [Interaction(InteractionTrigger.RightClick, "Close", RequiredEnvVars = new[] { nameof(Open) })]
        public void Toggle(Player player, InteractionTriggerInfo trigger, InteractionTarget target) => this.ToggleOpen();

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void PostInitialize()
        {
            SetState(Open);
            base.PostInitialize();
        }

        public override void SendInitialState(BSONObject bsonObj, INetObjectViewer viewer)
        {
            bsonObj["open"] = Open;
            bsonObj["closed"] = !Open;
            base.SendInitialState(bsonObj, viewer);
        }

        private void ToggleOpen()
        {
            if (Open)
                SetOpen(false);
            else
                SetOpen(true);
        }


        public void SetState(bool State)
        {
            SetOpen(State);
        }

        public void SetOpen(bool open)
        {
            SetAnimatedState("Open", open);
            Open = open;
            if (Open)
            {
                AudioManager.PlayAudio("Doors/DoorOpenSfx", Position);
                RPCManager.Call("Open", netEntity, null);
            }
            else
            {
                AudioManager.PlayAudio("Doors/DoorCloseSfx", Position);
                RPCManager.Call("Close", netEntity, null);
            }
        }
    }
}
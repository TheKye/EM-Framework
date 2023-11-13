using Eco.Core.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using Eco.Simulation;
using Eco.Simulation.WorldLayers;
using Eco.Simulation.WorldLayers.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using Eco.Shared.Localization;
using Eco.Gameplay.Items;
using Eco.Gameplay.Components.Storage;

namespace Eco.EM.Framework.Components
{
    [Serialized]
    [Priority(-2)]
    public abstract class BaseAnimalTrapComponent: WorldObjectComponent
    {
        // Trap enable and fail states
        public override bool Enabled => true;
        private StatusElement status;
        public virtual Func<object, bool> EnabledTest { get; set; } = (x) => true;
        public virtual LocString FailStatusMessage { get; } = Localizer.DoStr($"Trap is currently not operational.");

        // Internal Trap Slots
        protected PublicStorageComponent storage;

        // Amimal layers
        protected List<AnimalLayer> animalLayers;

        protected virtual void UpdateEnabled() => Parent.SetAnimatedState("Enabled", Enabled);
        public virtual void UpdateTrappingStatus() => status.SetStatusMessage(Enabled, Localizer.DoStr("Currently trapping animals."), FailStatusMessage);
        protected virtual void UpdateAnimalsInTrap(User user = null) => Parent.SetAnimatedState("AnimalsInTrap", !storage.Inventory.IsEmpty);

        protected virtual void UpdateTrap()
        {
            UpdateEnabled();
            UpdateTrappingStatus();
            UpdateAnimalsInTrap();
        }

        protected virtual void SetStorage() 
        {
            storage = Parent.GetComponent<PublicStorageComponent>();
            storage.Initialize(1);
            storage.Inventory.AddInvRestriction(new StackLimitRestriction(1));
            storage.Inventory.OnChanged.Add(UpdateAnimalsInTrap);
        }
        
        protected virtual void SetLayers(List<string> layers)
        {
            var layersList = new List<AnimalLayer>();
            foreach (var layerName in layers) layersList.Add((AnimalLayer)WorldLayerManager.Obj.SpeciesToLayers[EcoSim.AllSpecies.First(x => x.Name == layerName)]);
            animalLayers = layersList;
        }

        public virtual void Initialize(List<string> layers)
        {
            status = Parent.GetComponent<StatusComponent>().CreateStatusElement();
            SetStorage();
            WorldLayerSync.Obj.PreTickActions.Add(LayerTick);
            SetLayers(layers);
            UpdateTrap();
        }

        public virtual void LayerTick()
        {
            UpdateTrap();
            if (Enabled) { InteractWithLayers(); }           
        }

        // Actualy layer interaction for catching the animal
        protected abstract void InteractWithLayers();
    }
}

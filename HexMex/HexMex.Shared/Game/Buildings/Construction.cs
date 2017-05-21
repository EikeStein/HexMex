﻿using System.Linq;
using HexMex.Controls;

namespace HexMex.Game.Buildings
{
    public class Construction : Structure, IHasProgress
    {
        public BuildingConstructionFactory ConstructionFactory { get; }

        public bool IsConstructing { get; private set; }
        public float PassedConstructionTime { get; private set; }

        public float Progress { get; private set; }

        public Construction(HexagonNode position, BuildingConstructionFactory buildingConstructionFactory, World world) : base(position, world)
        {
            ConstructionFactory = buildingConstructionFactory;
            ResourceDirector.AllIngredientsArrived += StartConstructing;
            ResourceDirector.RequestIngredients(ConstructionFactory.StructureDescription.ConstructionCost.ResourceTypes.ToArray(), null);
        }

        public override void Render(ExtendedDrawNode drawNode)
        {
            var position = Position.GetWorldPosition(World.GameSettings.LayoutSettings.HexagonRadius, World.GameSettings.LayoutSettings.HexagonMargin);
            var radius = World.GameSettings.VisualSettings.ConstructionRadius;
            drawNode.DrawCircle(position, radius, World.GameSettings.VisualSettings.ColorCollection.GrayNormal, World.GameSettings.VisualSettings.StructureBorderThickness, World.GameSettings.VisualSettings.ColorCollection.White);
        }

        public sealed override void Update(float dt)
        {
            base.Update(dt);
            if (!IsConstructing)
                return;
            PassedConstructionTime += dt;
            Progress = PassedConstructionTime / ConstructionFactory.StructureDescription.ConstructionTime;
            if (PassedConstructionTime >= ConstructionFactory.StructureDescription.ConstructionTime)
            {
                PassedConstructionTime = 0;
                IsConstructing = false;
                var structure = ConstructionFactory.CreateFunction(Position, World);
                World.StructureManager.RemoveStructure(this);
                World.StructureManager.CreateStrucuture(structure);
            }
            OnRequiresRedraw();
        }

        private void StartConstructing(ResourceDirector arg1, ResourceType[] arg2)
        {
            IsConstructing = true;
        }
    }
}
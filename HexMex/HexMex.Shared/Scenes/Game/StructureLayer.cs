using System.Collections.Generic;
using HexMex.Controls;
using HexMex.Game;

namespace HexMex.Scenes.Game
{
    public class StructureLayer : TouchLayer
    {
        public World World { get; }
        private ExtendedDrawNode DrawNode { get; } = new ExtendedDrawNode();

        private bool RedrawRequested { get; set; }

        private List<Structure> Structures { get; } = new List<Structure>();

        public StructureLayer(World world, HexMexCamera camera) : base(camera)
        {
            World = world;
            world.StructureManager.StructureAdded += StructureAdded;
            world.StructureManager.StructureRemoved += StructureRemoved;
            world.StructureManager.StructureReplaced += StructureReplaced;
            Schedule(Update, 0.05f);
            AddChild(DrawNode);
        }

        private void StructureReplaced(StructureManager structureManager, Structure oldStructure, Structure newStructure)
        {
            StructureRemoved(structureManager, oldStructure);
            StructureAdded(structureManager, newStructure);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (!RedrawRequested)
                return;
            RedrawRequested = false;
            Render();
        }

        private void Render()
        {
            DrawNode.Clear();
            var settings = World.GameSettings;
            foreach (var structure in Structures)
            {
                structure.Render(DrawNode, structure.Position.GetWorldPosition(settings.LayoutSettings.HexagonRadius, settings.LayoutSettings.HexagonMargin), settings.VisualSettings.BuildingRadius);
            }
        }

        private void StructureAdded(StructureManager structureManager, Structure structure)
        {
            structure.RequiresRedraw += s => RedrawRequested = true;
            Structures.Add(structure);
            Render();
        }

        private void StructureRemoved(StructureManager structureManager, Structure structure)
        {
            Structures.Remove(structure);
            RedrawRequested = true;
            Render();
        }
    }
}
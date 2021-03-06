using System;
using CocosSharp;
using HexMex.Controls;

namespace HexMex.Game
{
    public abstract class Structure : IRenderable<Structure>, ICCUpdatable
    {
        public event Action<Structure> RequiresRedraw;

        public HexagonNode Position { get; }
        public World World { get; }

        public BuildingDescription Description { get; }

        public ResourceDirector ResourceDirector { get; }

        protected Structure(HexagonNode position, World world, BuildingDescription description)
        {
            World = world;
            Description = description;
            Position = position;
            ResourceDirector = new ResourceDirector(this);
            var hex1 = World.HexagonManager[position.Position1];
            var hex2 = World.HexagonManager[position.Position2];
            var hex3 = World.HexagonManager[position.Position3];
            hex1.Payout += OnAdjacentHexagonProvidedResource;
            hex2.Payout += OnAdjacentHexagonProvidedResource;
            hex3.Payout += OnAdjacentHexagonProvidedResource;
        }

        /// <summary>
        /// Get's called everytime a Resource arrives which destination was this Building.
        /// </summary>
        /// <param name="resource">The Resource that arrived.</param>
        public void OnResourceArrived(ResourcePackage resource)
        {
            ResourceDirector.ResourceArrived(resource);
        }

        public void OnAdjacentHexagonProvidedResource(Hexagon hexagon, ResourceType resourceType)
        {
            ResourceDirector.AdjacentHexagonProvidedResource(resourceType);
        }

        /// <summary>
        /// Get's called everytime a Resource passes the Node the Building is located at.
        /// </summary>
        /// <param name="resource">The Resource that passes through.</param>
        public virtual void OnResourcePassesThrough(ResourcePackage resource)
        {
            ResourceDirector.ResourcePassesThrough(resource);
        }

        public void Render(ExtendedDrawNode drawNode, CCPoint position, float radius)
        {
            var visualSettings = World.GameSettings.VisualSettings;
            var fillColorKey = Description.RenderInformation.FillColorKey;
            var borderColorKey = Description.RenderInformation.BorderColorKey;
            var fillColor = World.GameSettings.VisualSettings.ColorCollection.FromKey(fillColorKey);
            var borderColor = World.GameSettings.VisualSettings.ColorCollection.FromKey(borderColorKey);
            drawNode.DrawCircle(position,
                                radius,
                                fillColor,
                                visualSettings.StructureBorderThickness,
                                borderColor
                               );
        }

        public virtual void Update(float dt)
        {
        }

        protected internal void OnRequiresRedraw()
        {
            RequiresRedraw?.Invoke(this);
        }
    }
}
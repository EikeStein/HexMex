using System.Linq;
using CocosSharp;
using HexMex.Controls;

namespace HexMex.Game.Buildings
{
    public class IronExtractor : Building
    {
        public static BuildingDescription StructureDescription { get; } = BuildingDescriptionDatabase.Get<IronExtractor>();

        public IronExtractor(HexagonNode position, World world) : base(position, world, StructureDescription) { }

        public override void Render(ExtendedDrawNode drawNode, CCPoint position, float radius)
        {
            var visualSettings = World.GameSettings.VisualSettings;
            drawNode.DrawCircle(position,
                                radius,
                                visualSettings.ColorCollection.GrayLight,
                                visualSettings.StructureBorderThickness,
                                visualSettings.ColorCollection.White);
        }
    }
}
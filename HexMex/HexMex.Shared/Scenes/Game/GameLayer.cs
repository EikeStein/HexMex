﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using CocosSharp;
using HexMex.Controls;
using HexMex.Game;
using HexMex.Game.Buildings;

namespace HexMex.Scenes.Game
{
    public class GameLayer : CCLayerColor
    {
        public HexMexCamera HexMexCamera { get; }
        public GameTouchHandler TouchHandler { get; }

        public ReadOnlyCollection<TouchLayer> TouchLayers { get; }
        public World World { get; }

        private BuildMenu BuildMenu { get; }
        public StructureMenu StructureMenu { get; }

        public GameLayer(World world, HexMexCamera camera, CCColor4B color) : base(color)
        {
            HexMexCamera = camera;
            TouchHandler = new GameTouchHandler(this, HexMexCamera);
            World = world;
            var hexagonLayer = new HexagonLayer(World, HexMexCamera);
            var edgeLayer = new EdgeLayer(World, HexMexCamera);
            var resourcePackageLayer = new ResourcePackageLayer(World, HexMexCamera);
            var structureLayer = new StructureLayer(World, HexMexCamera);
            var controlLayer = new ButtonLayer(World, HexMexCamera);
            var statisticLayer = new StatisticLayer(World);
            var menuLayer = new MenuLayer(World, HexMexCamera);
            controlLayer.ConstructionRequested += (buttonLayer, buildButton) => ConstructionMenuRequested(buildButton, menuLayer);
            controlLayer.DisplayStructureRequested += (buttonLayer, structureButton) => DisplayStructureMenu(structureButton, menuLayer);

            var layers = new CCLayer[] { hexagonLayer, edgeLayer, resourcePackageLayer, structureLayer, controlLayer, statisticLayer, menuLayer };

            foreach (var layer in layers)
            {
                AddChild(layer);
            }

            TouchLayers = new ReadOnlyCollection<TouchLayer>(layers.OfType<TouchLayer>().Reverse().ToList());

            BuildMenu = new BuildMenu(World.UnlockManager, World.GameSettings.VisualSettings,World.GameSettings.LanguageSettings);
            BuildMenu.ConstructionRequested += ConstructBuilding;
            StructureMenu = new StructureMenu(World.GameSettings.VisualSettings, World);

            Schedule();
        }
        private void DisplayStructureMenu(StructureButton structureButton, MenuLayer menuLayer)
        {
            BuildMenu.TargetNode = structureButton.Structure.Position;
            StructureMenu.Structure = structureButton.Structure;
            menuLayer.DisplayMenu(StructureMenu);
        }

        private void ConstructionMenuRequested(BuildButton buildButton, MenuLayer menuLayer)
        {
            BuildMenu.TargetNode = buildButton.HexagonNode;
            menuLayer.DisplayMenu(BuildMenu);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            World.Update(dt);
        }

        private void ConstructBuilding(BuildMenu buildMenu, BuildingConstructionFactory selectedFactory)
        {
            if (World.StructureManager[buildMenu.TargetNode] != null)
                throw new InvalidOperationException("Spot has to be empty");
            var construction = new Construction(buildMenu.TargetNode, selectedFactory, World);
            World.StructureManager.CreateStrucuture(construction);
        }
    }
}
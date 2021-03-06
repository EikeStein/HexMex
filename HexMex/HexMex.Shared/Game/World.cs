﻿using System;
using System.Linq;
using CocosSharp;
using HexMex.Controls;
using HexMex.Game.Buildings;
using HexMex.Game.Settings;

namespace HexMex.Game
{
    public class World : ICCUpdatable
    {
        public event Action<World> Defeat;

        public event Action<World> Victory;
        public GameSpeed GameSpeed { get; set; } = GameSpeed.Normal;
        public ButtonManager ButtonManager { get; }
        public HexagonManager HexagonManager { get; }
        public bool IsInitialized { get; private set; }
        public CachedPathFinder PathFinder { get; }
        public ResourceManager ResourceManager { get; }
        public StructureManager StructureManager { get; }
        public GameSettings GameSettings { get; }
        public GlobalResourceManager GlobalResourceManager { get; }
        public UnlockManager UnlockManager { get; }
        public bool IsStopped { get; private set; }

        public World(GameSettings gameSettings)
        {
            GameSettings = gameSettings;
            StructureManager = new StructureManager();
            GlobalResourceManager = new GlobalResourceManager(GameSettings.GameplaySettings);
            HexagonManager = new HexagonManager(GameSettings.GameplaySettings);
            ButtonManager = new ButtonManager(GameSettings.LayoutSettings);
            UnlockManager = new UnlockManager(GlobalResourceManager, gameSettings.BuildingDescriptionDatabase);
            PathFinder = new CachedPathFinder(HexagonManager, StructureManager, GameSettings.GameplaySettings);
            ResourceManager = new ResourceManager(this);
            StructureManager.StructureAdded += StructureAdded;
            StructureManager.StructureRemoved += StructureRemoved;
            GlobalResourceManager.OutOfOxygen += grm => OnDefeat();
            GlobalResourceManager.ValueChanged += CheckWinCondition;
        }

        public void Initialize()
        {
            var p1 = HexagonPosition.Zero;
            var p2 = new HexagonPosition(0, 1, -1);
            var p3 = new HexagonPosition(1, 0, -1);

            HexagonManager.RevealHexagonAt(p1);
            HexagonManager.RevealHexagonAt(p2);
            HexagonManager.RevealHexagonAt(p3);

            StructureManager.CreateStrucuture(new Building(new HexagonNode(p1, p2, p3), this, GameSettings.BuildingDescriptionDatabase.ByNameKey("diamondExtractorName")));
            StructureManager.CreateStrucuture(new Building(new HexagonNode(new HexagonPosition(1, 1, -2), p2, p3), this, GameSettings.BuildingDescriptionDatabase.ByNameKey("haborName")));
            StructureManager.CreateStrucuture(new Building(new HexagonNode(new HexagonPosition(1, 1, -2), new HexagonPosition(2, 0, -2), p3), this, GameSettings.BuildingDescriptionDatabase.ByNameKey("coalPowerPlantName")));

            IsInitialized = true;
        }

        public void OnVictory()
        {
            Victory?.Invoke(this);
        }
        public void Stop()
        {
            IsStopped = true;
        }

        public void Update(float dt)
        {
            if (!IsInitialized || IsStopped)
                return;
            var gameSpeed = (float)GameSpeed / 10;
            dt *= gameSpeed;
            HexagonManager.Update(dt);
            ResourceManager.Update(dt);
            StructureManager.Update(dt);
        }

        private void CheckWinCondition(GlobalResourceManager globalResourceManager)
        {
            if (globalResourceManager.EnvironmentResource.WinStep >= GameSettings.GameplaySettings.PalastWinSteps)
            {
                OnVictory();
            }
        }

        private void OnDefeat()
        {
            Defeat?.Invoke(this);
        }

        private void StructureAdded(StructureManager manager, Structure structure)
        {
            if (ButtonManager[structure.Position] != null)
                ButtonManager.RemoveButton(ButtonManager[structure.Position]);
            ButtonManager.AddButton(new StructureButton(GameSettings, structure), structure.Position);
            if (!(structure is Construction))
            {
                foreach (var adjacentHexagonNode in structure.Position.GetAccessibleAdjacentHexagonNodes(HexagonManager))
                {
                    var p1 = adjacentHexagonNode.Position1;
                    var p2 = adjacentHexagonNode.Position2;
                    var p3 = adjacentHexagonNode.Position3;
                    if (HexagonManager.GetHexagonAtPosition(p1) == null)
                        HexagonManager.RevealHexagonAt(p1);
                    if (HexagonManager.GetHexagonAtPosition(p2) == null)
                        HexagonManager.RevealHexagonAt(p2);
                    if (HexagonManager.GetHexagonAtPosition(p3) == null)
                        HexagonManager.RevealHexagonAt(p3);

                    if (StructureManager[adjacentHexagonNode] == null && ButtonManager[adjacentHexagonNode] == null)
                    {
                        var button = new BuildButton(GameSettings, adjacentHexagonNode);
                        ButtonManager.AddButton(button, adjacentHexagonNode);
                    }
                    /*if (!EdgeManager.ContainsEdge(structure.Position, adjacentHexagonNode))
                        EdgeManager.AddEdge(structure.Position, adjacentHexagonNode);*/
                }
            }
        }

        // TODO Implement
        private void StructureRemoved(StructureManager structureManager, Structure structure)
        {
            if (ButtonManager[structure.Position] != null)
                ButtonManager.RemoveButton(ButtonManager[structure.Position]);
            var hexagonNodes = structure.Position.GetAccessibleAdjacentHexagonNodes(HexagonManager).Where(s => StructureManager[s] is Building).ToArray();
            if (hexagonNodes.Any())
                ButtonManager.AddButton(new BuildButton(GameSettings, structure.Position), structure.Position);
        }
    }
}
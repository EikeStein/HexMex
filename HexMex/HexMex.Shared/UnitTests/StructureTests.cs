﻿// ReSharper disable UnusedMember.Global

using System.Linq;
using System.Reflection;
using HexMex.Game;
using HexMex.Game.Buildings;

namespace HexMex.UnitTests
{
    [TestClass]
    public class StructureTests
    {
        [TestMethod]
        public void StructuresIncludeBuildingInformationTest()
        {
            var allTypes = typeof(Building).Assembly.GetTypes();
            var dirtyTypes = (from type in allTypes
                              where !type.IsAbstract
                              where type.IsSubclassOf(typeof(Building))
                              let property = type.GetProperty("StructureDescription", BindingFlags.Static | BindingFlags.Public)
                              where property == null || property.PropertyType.IsSubclassOf(typeof(StructureDescription))
                              select new {type.Name, Type = type}).ToArray();
            var dirtyNames = string.Join(", ", dirtyTypes.Select(a => a.Name));
            if (dirtyTypes.Any())
                throw new AssertException(dirtyNames);
        }
    }
}
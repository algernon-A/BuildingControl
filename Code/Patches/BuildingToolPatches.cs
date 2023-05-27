// <copyright file="BuildingToolPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl.Patches
{
    using System;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmomy patches for BuildingTool to set building settings when a building is created.
    /// </summary>
    [HarmonyPatch(typeof(BuildingTool))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    internal static class BuildingToolPatches
    {
        /// <summary>
        /// Harmony postfix for BuildingManager.CreateBuilding apply current building settings when a building is created.
        /// </summary>
        /// <param name="__result">Method result (building ID).</param>
        [HarmonyPatch(
            "CreateBuilding",
            new Type[] { typeof(BuildingInfo), typeof(Vector3), typeof(float), typeof(int), typeof(bool), typeof(bool) },
            new ArgumentType[] { ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
        [HarmonyPostfix]
        internal static void CreateBuildingPostfix(ref ushort __result)
        {
            if (__result != 0)
            {
                BuildingData.Instance.SetBuildingSetting(__result);
            }
        }
    }
}

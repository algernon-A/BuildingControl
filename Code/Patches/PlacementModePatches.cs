// <copyright file="PlacementModePatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl.Patches
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using AlgernonCommons;
    using HarmonyLib;

    /// <summary>
    /// Harmomy patches for building instances to implement terrain and surface overrides.
    /// </summary>
    [HarmonyPatch]
    internal static class PlacementModePatches
    {
        /// <summary>
        /// Determines list of target methods to patch.
        /// </summary>
        /// <returns>List of target methods to patch.</returns>
        internal static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(BuildingAI), nameof(BuildingAI.CheckBuildPosition));
            yield return AccessTools.Method(typeof(CommonBuildingAI), nameof(CommonBuildingAI.GetTerrainLimits));
            yield return AccessTools.Method(typeof(BuildingTool), "OnToolGUI");
            yield return AccessTools.Method(typeof(BuildingTool), "OnToolUpdate");
            yield return AccessTools.Method(typeof(BuildingTool), "OnToolLateUpdate");
            yield return AccessTools.Method(typeof(BuildingTool), "CreateBuilding");
            yield return AccessTools.Method(typeof(BuildingTool), "CheckSpaceImpl");
            yield return AccessTools.Method(typeof(BuildingTool), nameof(BuildingTool.SimulationStep));
            yield return AccessTools.Method(typeof(DefaultTool), "CheckPlacementErrors");
            yield return AccessTools.Method(typeof(DefaultTool), "CheckBuilding");
        }

        /// <summary>
        /// Harmony transpiler for to replace references to BuildingInfo terrain and surface fields with building data lookups.
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <returns>Modified ILCode.</returns>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Target field.
            FieldInfo m_placementMode = AccessTools.Field(typeof(BuildingInfo), nameof(BuildingInfo.m_placementMode));

            // Replacement methods.
            MethodInfo getFlattenTerrain = AccessTools.Method(typeof(PlacementModePatches), nameof(GetPlacementMode));

            // Iterate through all instructions.
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.LoadsField(m_placementMode))
                {
                    Logging.Message("found m_placementMode");

                    // In all target methods argument 1 is buildingID.
                    yield return new CodeInstruction(OpCodes.Call, getFlattenTerrain);

                    // Skip original instruction.
                    continue;
                }

                yield return instruction;
            }
        }

        /// <summary>
        /// Returns the current placement mode..
        /// </summary>
        /// <param name="buildingInfo"><see cref="BuildingInfo"/> prefab.</param>
        /// <returns><see cref="BuildingInfo.PlacementMode"/> replacement value.</returns>
        private static BuildingInfo.PlacementMode GetPlacementMode(BuildingInfo buildingInfo)
        {
            return BuildingData.Instance.OverridePlacement ? BuildingData.Instance.Placement : buildingInfo.m_placementMode;
        }
    }
}

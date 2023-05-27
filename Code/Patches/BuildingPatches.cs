// <copyright file="BuildingPatches.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using AlgernonCommons;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmomy patches for building instances to implement terrain and surface overrides.
    /// </summary>
    [HarmonyPatch]
    internal static class BuildingPatches
    {
        /// <summary>
        /// Determines list of target methods to patch.
        /// </summary>
        /// <returns>List of target methods to patch.</returns>
        internal static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Building), nameof(Building.UpdateBuilding));
            yield return AccessTools.Method(typeof(Building), nameof(Building.AfterTerrainUpdated));
            yield return AccessTools.Method(
                typeof(Building),
                nameof(Building.TerrainUpdated),
                new Type[] { typeof(BuildingInfo), typeof(ushort), typeof(Vector3), typeof(float), typeof(int), typeof(int), typeof(float), typeof(float), typeof(float), typeof(float), typeof(bool) });
        }

        /// <summary>
        /// Harmony transpiler for to replace references to BuildingInfo terrain and surface fields with building data lookups.
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <returns>Modified ILCode.</returns>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Target field infos.
            FieldInfo m_flattenTerrain = AccessTools.Field(typeof(BuildingInfo), nameof(BuildingInfo.m_flattenTerrain));
            FieldInfo m_flattenFullArea = AccessTools.Field(typeof(BuildingInfo), nameof(BuildingInfo.m_flattenFullArea));
            FieldInfo m_fullGravel = AccessTools.Field(typeof(BuildingInfo), nameof(BuildingInfo.m_fullGravel));
            FieldInfo m_fullPavement = AccessTools.Field(typeof(BuildingInfo), nameof(BuildingInfo.m_fullPavement));

            // Replacement methods.
            MethodInfo getFlattenTerrain = AccessTools.Method(typeof(BuildingPatches), nameof(GetFlattenTerrain));
            MethodInfo getFlattenFullArea = AccessTools.Method(typeof(BuildingPatches), nameof(GetFlattenFullArea));
            MethodInfo getFullGravel = AccessTools.Method(typeof(BuildingPatches), nameof(GetFullGravel));
            MethodInfo getFullPavement = AccessTools.Method(typeof(BuildingPatches), nameof(GetFullPavement));

            // Iterate through all instructions.
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.LoadsField(m_flattenTerrain))
                {
                    Logging.Message("found m_flattenTerrain");

                    // In all target methods argument 1 is buildingID.
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, getFlattenTerrain);

                    // Skip original instruction.
                    continue;
                }
                else if (instruction.LoadsField(m_flattenFullArea))
                {
                    Logging.Message("found m_flattenFullArea");

                    // In all target methods argument 1 is buildingID.
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, getFlattenFullArea);

                    // Skip original instruction.
                    continue;
                }
                else if (instruction.LoadsField(m_fullGravel))
                {
                    Logging.Message("found m_fullGravel");

                    // In all target methods argument 1 is buildingID.
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, getFullGravel);

                    // Skip original instruction.
                    continue;
                }
                else if (instruction.LoadsField(m_fullPavement))
                {
                    Logging.Message("found m_fullPavement");

                    // In all target methods argument 1 is buildingID.
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, getFullPavement);

                    // Skip original instruction.
                    continue;
                }

                yield return instruction;
            }
        }

        /// <summary>
        /// Returns the appropriate m_flattenTerrain setting for the given building ID.
        /// </summary>
        /// <param name="buildingInfo"><see cref="BuildingInfo"/> prefab.</param>
        /// <param name="buildingID">Building ID.</param>
        /// <returns><see cref="BuildingInfo.m_flattenTerrain"/> replacement value.</returns>
        private static bool GetFlattenTerrain(BuildingInfo buildingInfo, ushort buildingID)
        {
            switch (BuildingData.Instance.GetTerrainMode(buildingID))
            {
                default:
                case BuildingRecord.TerrainMode.Default:
                    return buildingInfo.m_flattenTerrain;
                case BuildingRecord.TerrainMode.Flatten:
                case BuildingRecord.TerrainMode.FlattenAll:
                    return true;
                case BuildingRecord.TerrainMode.IgnoreTerrain:
                    return false;
            }
        }

        /// <summary>
        /// Returns the appropriate m_flattenFullArea setting for the given building ID.
        /// </summary>
        /// <param name="buildingInfo"><see cref="BuildingInfo"/> prefab.</param>
        /// <param name="buildingID">Building ID.</param>
        /// <returns><see cref="BuildingInfo.m_flattenFullArea"/> replacement value.</returns>
        private static bool GetFlattenFullArea(BuildingInfo buildingInfo, ushort buildingID)
        {
            switch (BuildingData.Instance.GetTerrainMode(buildingID))
            {
                default:
                case BuildingRecord.TerrainMode.Default:
                    return buildingInfo.m_flattenFullArea;
                case BuildingRecord.TerrainMode.FlattenAll:
                    return true;
                case BuildingRecord.TerrainMode.Flatten:
                case BuildingRecord.TerrainMode.IgnoreTerrain:
                    return false;
            }
        }

        /// <summary>
        /// Returns the appropriate m_fullGravel setting for the given building ID.
        /// </summary>
        /// <param name="buildingInfo"><see cref="BuildingInfo"/> prefab.</param>
        /// <param name="buildingID">Building ID.</param>
        /// <returns><see cref="BuildingInfo.m_fullGravel"/> replacement value.</returns>
        private static bool GetFullGravel(BuildingInfo buildingInfo, ushort buildingID)
        {
            switch (BuildingData.Instance.GetSurfaceTexture(buildingID))
            {
                default:
                case BuildingRecord.SurfaceTexture.Default:
                    return buildingInfo.m_fullGravel;
                case BuildingRecord.SurfaceTexture.Gravel:
                    return true;
                case BuildingRecord.SurfaceTexture.None:
                case BuildingRecord.SurfaceTexture.Pavement:
                    return false;
            }
        }

        /// <summary>
        /// Returns the appropriate m_fullPavement setting for the given building ID.
        /// </summary>
        /// <param name="buildingInfo"><see cref="BuildingInfo"/> prefab.</param>
        /// <param name="buildingID">Building ID.</param>
        /// <returns><see cref="BuildingInfo.m_fullPavement"/> replacement value.</returns>
        private static bool GetFullPavement(BuildingInfo buildingInfo, ushort buildingID)
        {
            switch (BuildingData.Instance.GetSurfaceTexture(buildingID))
            {
                default:
                case BuildingRecord.SurfaceTexture.Default:
                    return buildingInfo.m_fullGravel;
                case BuildingRecord.SurfaceTexture.Pavement:
                    return true;
                case BuildingRecord.SurfaceTexture.None:
                case BuildingRecord.SurfaceTexture.Gravel:
                    return false;
            }
        }
    }
}

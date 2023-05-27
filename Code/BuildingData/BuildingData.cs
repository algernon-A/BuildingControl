// <copyright file="BuildingData.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl
{
    using System.Collections.Generic;

    /// <summary>
    /// Class to manage building data records.
    /// </summary>
    internal class BuildingData
    {
        // Buildng data records.
        private readonly Dictionary<ushort, BuildingRecord> _buildingRecords = new Dictionary<ushort, BuildingRecord>();

        /// <summary>
        /// Gets or sets the active instance.
        /// </summary>
        internal static BuildingData Instance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a placment mode override is in effect.
        /// </summary>
        internal bool OverridePlacement { get; set; }

        /// <summary>
        /// Gets or sets the placement mode override.
        /// </summary>
        internal BuildingInfo.PlacementMode Placement { get; set; }

        /// <summary>
        /// Gets or sets the current surface texture override.
        /// </summary>
        internal BuildingRecord.SurfaceTexture SurfaceTexture { get; set; } = BuildingRecord.SurfaceTexture.Default;

        /// <summary>
        /// Gets or sets the current terrain mode override.
        /// </summary>
        internal BuildingRecord.TerrainMode TerrainMode { get; set; } = BuildingRecord.TerrainMode.Default;

        /// <summary>
        /// Sets the record for the given building to the current override values.
        /// </summary>
        /// <param name="buildingID">Building ID.</param>
        internal void SetBuildingSetting(ushort buildingID)
        {
            // Ignore default settings.
            if (!OverridePlacement && SurfaceTexture == BuildingRecord.SurfaceTexture.Default && TerrainMode == BuildingRecord.TerrainMode.Default)
            {
                AlgernonCommons.Logging.Message("no settings for building ", buildingID);

                // Default setting - remove any existing record.
                _buildingRecords.Remove(buildingID);
                return;
            }

            AlgernonCommons.Logging.Message("new settings for building ", buildingID);

            // Create new record and add to dictionary.
            BuildingRecord newRecord = new BuildingRecord
            {
                OverridePlacementMode = OverridePlacement,
                PlacementMode = Placement,
                SurfaceOverride = SurfaceTexture,
                TerrainOverride = TerrainMode,
            };

            _buildingRecords[buildingID] = newRecord;
        }

        /// <summary>
        /// Clears the record for the given building.
        /// </summary>
        /// <param name="buildingID">Building ID.</param>
        internal void ClearBuildingSetting(ushort buildingID) => _buildingRecords.Remove(buildingID);

        /// <summary>
        /// Gets the terrain override for the given building.
        /// </summary>
        /// <param name="buildingID">Building ID.</param>
        /// <returns>Terrain override (<see cref="BuildingRecord.TerrainMode.Default"/> if no active record exists).</returns>
        internal BuildingRecord.TerrainMode GetTerrainMode(ushort buildingID)
        {
            // Get stored building record.
            if (_buildingRecords.TryGetValue(buildingID, out BuildingRecord record))
            {
                return record.TerrainOverride;
            }

            // If we got here, no building record was found; return default.
            return BuildingRecord.TerrainMode.Default;
        }

        /// <summary>
        /// Gets the surface override for the given building.
        /// </summary>
        /// <param name="buildingID">Building ID.</param>
        /// <returns>Surface override (<see cref="BuildingRecord.SurfaceTexture.Default"/> if no active record exists).</returns>
        internal BuildingRecord.SurfaceTexture GetSurfaceTexture(ushort buildingID)
        {
            // Get stored building record.
            if (_buildingRecords.TryGetValue(buildingID, out BuildingRecord record))
            {
                return record.SurfaceOverride;
            }

            // If we got here, no building record was found; return default.
            return BuildingRecord.SurfaceTexture.Default;
        }
    }
}

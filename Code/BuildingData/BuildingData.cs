// <copyright file="BuildingData.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl
{
    using System.Collections.Generic;
    using System.IO;
    using AlgernonCommons;
    using ColossalFramework;
    using ColossalFramework.IO;
    using static BuildingRecord;

    /// <summary>
    /// Class to manage building data records.
    /// </summary>
    internal class BuildingData
    {
        // Instance reference.
        private static BuildingData s_instance;

        // Buildng data records.
        private readonly Dictionary<ushort, BuildingRecord> _buildingRecords = new Dictionary<ushort, BuildingRecord>();


        /// <summary>
        /// Gets the active instance.
        /// </summary>
        internal static BuildingData Instance
        {
            get
            {
                // Initialize new instance if one doesn't already exist.
                if (s_instance == null)
                {
                    s_instance = new BuildingData();

                    // See if this save contains any Building Control data.
                    if (Singleton<SimulationManager>.instance.m_serializableDataStorage.TryGetValue(Serializer.DataID, out byte[] data))
                    {
                        // Yes - deserialize (making sure that all data is ready before returning instance reference).
                        using (MemoryStream stream = new MemoryStream(data))
                        {
                            Logging.Message("found exsting savegame data");
                            DataSerializer.Deserialize<DataContainer>(stream, DataSerializer.Mode.Memory);
                        }
                    }
                }

                return s_instance;
            }
        }

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
        internal SurfaceTexture SurfaceTexture { get; set; } = SurfaceTexture.Default;

        /// <summary>
        /// Gets or sets the current terrain mode override.
        /// </summary>
        internal TerrainMode TerrainMode { get; set; } = TerrainMode.Default;

        /// <summary>
        /// Sets the record for the given building to the current override values.
        /// </summary>
        /// <param name="buildingID">Building ID.</param>
        internal void SetBuildingSetting(ushort buildingID)
        {
            // Ignore default settings.
            if (!OverridePlacement && SurfaceTexture == SurfaceTexture.Default && TerrainMode == TerrainMode.Default)
            {
                // Default setting - remove any existing record.
                _buildingRecords.Remove(buildingID);
                return;
            }

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
        /// <returns>Terrain override (<see cref="TerrainMode.Default"/> if no active record exists).</returns>
        internal TerrainMode GetTerrainMode(ushort buildingID)
        {
            // Get stored building record.
            if (_buildingRecords.TryGetValue(buildingID, out BuildingRecord record))
            {
                return record.TerrainOverride;
            }

            // If we got here, no building record was found; return default.
            return TerrainMode.Default;
        }

        /// <summary>
        /// Gets the surface override for the given building.
        /// </summary>
        /// <param name="buildingID">Building ID.</param>
        /// <returns>Surface override (<see cref="SurfaceTexture.Default"/> if no active record exists).</returns>
        internal SurfaceTexture GetSurfaceTexture(ushort buildingID)
        {
            // Get stored building record.
            if (_buildingRecords.TryGetValue(buildingID, out BuildingRecord record))
            {
                return record.SurfaceOverride;
            }

            // If we got here, no building record was found; return default.
            return SurfaceTexture.Default;
        }

        /// <summary>
        /// Serialise to savegame.
        /// </summary>
        /// <param name="serializer">Data serializer.</param>
        internal void Serialize(DataSerializer serializer)
        {
            // Write data length.
            int dataLength = _buildingRecords.Count;
            serializer.WriteInt32(dataLength);

            // Serialize entries.
            foreach (KeyValuePair<ushort, BuildingRecord> entry in _buildingRecords)
            {
                serializer.WriteUInt16(entry.Key);
                serializer.WriteBool(entry.Value.OverridePlacementMode);
                serializer.WriteInt32((int)entry.Value.PlacementMode);
                serializer.WriteInt8((byte)entry.Value.SurfaceOverride);
                serializer.WriteInt8((byte)entry.Value.TerrainOverride);
            }

            Logging.Message("wrote ", dataLength, " records");
        }

        /// <summary>
        /// Deserialise from savegame.
        /// </summary>
        /// <param name="serializer">Data serializer.</param>
        internal void Deserialize(DataSerializer serializer)
        {
            // Read data length.
            int dataLength = serializer.ReadInt32();

            // Deerialize entries.
            _buildingRecords.Clear();
            for (int i = 0; i < dataLength; ++i)
            {
                _buildingRecords.Add(
                    (ushort)serializer.ReadUInt16(),
                    new BuildingRecord
                    {
                        OverridePlacementMode = serializer.ReadBool(),
                        PlacementMode = (BuildingInfo.PlacementMode)serializer.ReadInt32(),
                        SurfaceOverride = (SurfaceTexture)serializer.ReadUInt8(),
                        TerrainOverride = (TerrainMode)serializer.ReadUInt8(),
                    });
            }

            Logging.Message("read ", dataLength, " records");
        }
    }
}

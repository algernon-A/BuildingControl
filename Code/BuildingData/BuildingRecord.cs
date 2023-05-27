// <copyright file="BuildingRecord.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl
{
    /// <summary>
    /// Struct to store building data.
    /// </summary>
    internal struct BuildingRecord
    {
        /// <summary>
        /// Gets or sets a value indicating whether placement mode should be overriden.
        /// </summary>
        public bool OverridePlacementMode;

        /// <summary>
        /// Placement mode override.
        /// </summary>
        public BuildingInfo.PlacementMode PlacementMode;

        /// <summary>
        /// Surface texture override.
        /// </summary>
        public SurfaceTexture SurfaceOverride;

        /// <summary>
        /// Terrain behavior override.
        /// </summary>
        public TerrainMode TerrainOverride;

        /// <summary>
        /// Surface texture.
        /// </summary>
        public enum SurfaceTexture : byte
        {
            /// <summary>
            /// Default behavior - no override.
            /// </summary>
            Default = 0,

            /// <summary>
            /// No surface texture.
            /// </summary>
            None = 1,

            /// <summary>
            /// Force gravel surface texture.
            /// </summary>
            Gravel = 2,

            /// <summary>
            /// Force pavement surface texture.
            /// </summary>
            Pavement = 3,
        }

        /// <summary>
        /// Terrain behavior.
        /// </summary>
        public enum TerrainMode : byte
        {
            /// <summary>
            /// Default behavior - no override.
            /// </summary>
            Default = 0,

            /// <summary>
            /// Ignore terrain.
            /// </summary>
            IgnoreTerrain = 1,

            /// <summary>
            /// Force terrain flattening for building.
            /// </summary>
            Flatten = 2,

            /// <summary>
            /// Force terrain flattening for entire lot.
            /// </summary>
            FlattenAll = 3,
        }
    }
}

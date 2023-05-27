// <copyright file="BuildingExtension.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl
{
    using ICities;

    /// <summary>
    /// Building extension method class.
    /// </summary>
    public class BuildingExtension : BuildingExtensionBase
    {
        /// <summary>
        /// Checks to see if a released building has active setttings, and if so, clears them.
        /// Called by the game when a building is released.
        /// </summary>
        /// <param name="id">Building ID.</param>
        public override void OnBuildingReleased(ushort id) => BuildingData.Instance.ClearBuildingSetting(id);
    }
}

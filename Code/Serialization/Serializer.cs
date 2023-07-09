// <copyright file="Serializer.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl
{
    using System.IO;
    using AlgernonCommons;
    using ColossalFramework.IO;
    using ICities;

    /// <summary>
    /// Handles savegame data saving and loading.
    /// </summary>
    public sealed class Serializer : SerializableDataExtensionBase
    {
        /// <summary>
        /// Current data version.
        /// </summary>
        internal const int CurrentDataVersion = 0;

        /// <summary>
        /// Unique data ID for savegame data.
        /// </summary>
        internal const string DataID = "BuildingControl";

        /// <summary>
        /// Serializes data to the savegame.
        /// Called by the game on save.
        /// </summary>
        public override void OnSaveData()
        {
            base.OnSaveData();

            using (MemoryStream stream = new MemoryStream())
            {
                // Serialise savegame settings.
                DataSerializer.Serialize(stream, DataSerializer.Mode.Memory, CurrentDataVersion, new DataContainer());

                // Write to savegame.
                serializableDataManager.SaveData(DataID, stream.ToArray());

                Logging.Message("wrote ", stream.Length);
            }
        }

        /// <summary>
        /// Deserializes data from a savegame (or initialises new data structures when none available).
        /// Called by the game on load (including a new game).
        /// </summary>
        public override void OnLoadData()
        {
            // Nothing here - deserialization is called via BuildingData instance accessor, as default game deserialization happens too late.
        }
    }
}
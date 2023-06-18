// <copyright file="ModSettings.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl
{
    using System.IO;
    using System.Xml.Serialization;
    using AlgernonCommons.UI;
    using AlgernonCommons.XML;

    /// <summary>
    /// Global mod settings.
    /// </summary>
    [XmlRoot("BuildingControl")]
    public sealed class ModSettings : SettingsXMLBase
    {
        // Settings file name.
        [XmlIgnore]
        private static readonly string SettingsFileName = "BuildingControl.xml";

        // User settings directory.
        [XmlIgnore]
        private static readonly string UserSettingsDir = ColossalFramework.IO.DataLocation.localApplicationData;

        // Full userdir settings file name.
        [XmlIgnore]
        private static readonly string SettingsFile = Path.Combine(UserSettingsDir, SettingsFileName);

        /// <summary>
        /// Gets or sets the panel's saved X-position.
        /// </summary>
        [XmlElement("ControlPanelX")]
        public float ControlPanelX { get => StandalonePanelManager<ControlPanel>.LastSavedXPosition; set => StandalonePanelManager<ControlPanel>.LastSavedXPosition = value; }

        /// <summary>
        /// Gets or sets the panel's saved Y-position.
        /// </summary>
        [XmlElement("ControlPanelY")]
        public float ControlPanelY { get => StandalonePanelManager<ControlPanel>.LastSavedYPosition; set => StandalonePanelManager<ControlPanel>.LastSavedYPosition = value; }

        /// <summary>
        /// Loads settings from file.
        /// </summary>
        internal static void Load() => XMLFileUtils.Load<ModSettings>(SettingsFile);

        /// <summary>
        /// Saves settings to file.
        /// </summary>
        internal static void Save() => XMLFileUtils.Save<ModSettings>(SettingsFile);
    }
}
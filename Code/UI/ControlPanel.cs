﻿// <copyright file="ControlPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Control button panel.
    /// </summary>
    internal class ControlPanel : StandalonePanel
    {
        // Layout constants.
        private const float TitleHeight = 30f;
        private const float LabelHeight = 15f;
        private const float DropDownHeight = 25f;
        private const float ControlWidth = 200f;
        private const float CalculatedPanelWidth = ControlWidth + (Margin * 2f);
        private const float PlacementLabelY = TitleHeight + Margin;
        private const float PlacementMenuY = PlacementLabelY + LabelHeight;
        private const float SurfaceLabelY = PlacementMenuY + DropDownHeight + Margin;
        private const float SurfaceMenuY = SurfaceLabelY + LabelHeight;
        private const float TerrainLabelY = SurfaceMenuY + DropDownHeight + Margin;
        private const float TerrainMenuY = TerrainLabelY + LabelHeight;
        private const float CalculatedPanelHeight = TerrainMenuY + DropDownHeight + Margin;

        private readonly string[] _placementModeNames = new string[]
        {
            Translations.Translate("DEFAULT"),
            Translations.Translate("ROADSIDE"),
            Translations.Translate("SHORE"),
            Translations.Translate("ONWATER"),
            Translations.Translate("ONGROUND"),
            Translations.Translate("ONSURFACE"),
            Translations.Translate("ONTERRAIN"),
            Translations.Translate("SHORE_GROUND"),
            Translations.Translate("PATH_GROUND"),
            Translations.Translate("CONCORSE"),
        };

        private readonly string[] _surfaceModeNames = new string[]
        {
            Translations.Translate("DEFAULT"),
            Translations.Translate("NO_SURFACE"),
            Translations.Translate("GRAVEL"),
            Translations.Translate("PAVEMENT"),
        };

        private readonly string[] _terrainModeNames = new string[]
        {
            Translations.Translate("DEFAULT"),
            Translations.Translate("FLATTEN_OFF"),
            Translations.Translate("FLATTEN_BUILDING"),
            Translations.Translate("FLATTEN_ALL"),
        };

        // Panel components.

        /// <summary>
        /// Gets the panel width.
        /// </summary>
        public override float PanelWidth => CalculatedPanelWidth;

        /// <summary>
        /// Gets the panel height.
        /// </summary>
        public override float PanelHeight => CalculatedPanelHeight;

        /// <summary>
        /// Gets the panel's title.
        /// </summary>
        protected override string PanelTitle => Translations.Translate("MOD_NAME");

        /// <summary>
        /// Called by Unity before the first frame.
        /// Used to perform setup.
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Placement mode dropdown.
            UILabels.AddLabel(this, Margin, PlacementLabelY, Translations.Translate("PLACEMENT"), textScale: 0.8f);
            UIDropDown placementDropDown = UIDropDowns.AddDropDown(this, Margin, PlacementMenuY, ControlWidth);
            placementDropDown.BringToFront();
            placementDropDown.items = _placementModeNames;
            if (BuildingData.Instance.OverridePlacement)
            {
                placementDropDown.selectedIndex = (int)BuildingData.Instance.Placement + 1;
            }
            else
            {
                placementDropDown.selectedIndex = 0;
            }

            placementDropDown.eventSelectedIndexChanged += (c, index) =>
            {
                BuildingData.Instance.OverridePlacement = index != 0;
                BuildingData.Instance.Placement = (BuildingInfo.PlacementMode)(index - 1);
            };

            // Surface texture dropdown.
            UILabels.AddLabel(this, Margin, SurfaceLabelY, Translations.Translate("SURFACE"), textScale: 0.8f);
            UIDropDown surfaceDropDown = UIDropDowns.AddDropDown(this, Margin, SurfaceMenuY, ControlWidth);
            surfaceDropDown.BringToFront();
            surfaceDropDown.items = _surfaceModeNames;
            surfaceDropDown.selectedIndex = (int)BuildingData.Instance.SurfaceTexture;
            surfaceDropDown.eventSelectedIndexChanged += (c, index) =>
            {
                BuildingData.Instance.SurfaceTexture = (BuildingRecord.SurfaceTexture)index;
            };

            // Terrain flattening dropdown.
            UILabels.AddLabel(this, Margin, TerrainLabelY, Translations.Translate("FLATTEN"), textScale: 0.8f);
            UIDropDown terrainDropDown = UIDropDowns.AddDropDown(this, Margin, TerrainMenuY, ControlWidth);
            terrainDropDown.BringToFront();
            terrainDropDown.items = _terrainModeNames;
            terrainDropDown.selectedIndex = (int)BuildingData.Instance.TerrainMode;
            terrainDropDown.eventSelectedIndexChanged += (c, index) =>
            {
                BuildingData.Instance.TerrainMode = (BuildingRecord.TerrainMode)index;
            };

            terrainDropDown.eventDropdownOpen += (UIDropDown c, UIListBox p, ref bool o) =>
            {
                this.BringToFront();
                c.BringToFront();
                p.BringToFront();
            };

            surfaceDropDown.eventDropdownOpen += (UIDropDown c, UIListBox p, ref bool o) =>
            {
                this.BringToFront();
                c.BringToFront();
                p.BringToFront();
            };

            placementDropDown.eventDropdownOpen += (UIDropDown c, UIListBox p, ref bool o) =>
            {
                this.BringToFront();
                c.BringToFront();
                p.BringToFront();
            };
        }

        /// <summary>
        /// Applies the panel's default position.
        /// </summary>
        public override void ApplyDefaultPosition()
        {
            // Set position.
            UIComponent optionsBar = GameObject.Find("OptionsBar").GetComponent<UIComponent>();
            absolutePosition = optionsBar.absolutePosition - new Vector3(0f, CalculatedPanelHeight + Margin);
        }
    }
}

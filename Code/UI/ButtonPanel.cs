// <copyright file="ButtonPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace BuildingControl
{
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Control button panel.
    /// </summary>
    internal class ButtonPanel : StandalonePanelBase
    {
        // Layout constants.
        private const float ButtonSize = 36f;
        private const float ButtonSpacing = 2f;

        // Panel settings.
        private static bool s_showButtons = true;
        private static bool s_transparentUI = false;

        private readonly string[] _placementModeNames = new string[]
        {
            "Default",
            "Roadside",
            "Shoreline",
            "On water",
            "On ground",
            "On surface",
            "On terrain",
            "Shoreline or ground",
            "Pathside or ground",
            "Concourse",
        };

        private readonly string[] _surfaceModeNames = new string[]
        {
            "Default",
            "None",
            "Gravel",
            "Pavement",
        };

        private readonly string[] _terrainModeNames = new string[]
        {
            "Default",
            "Leave terrain",
            "Flatten building area",
            "Flatten whole area",
        };

        // Panel components.
        private UIMultiStateButton _placementButton;

        // Dragging.
        private bool _dragging = false;
        private Vector3 _lastDragPosition;

        /// <summary>
        /// Gets or sets a value indicating whether the button panel should be shown.
        /// </summary>
        public static bool ShowButtons
        {
            get => s_showButtons;

            set
            {
                // Don't do anything if no change.
                if (value != s_showButtons)
                {
                    s_showButtons = value;

                    // Showing - create panel if in-game.
                    if (value)
                    {
                        if (Loading.IsLoaded)
                        {
                            StandalonePanelManager<ButtonPanel>.Create();
                        }
                    }
                    else
                    {
                        // Hiding - close status panel if open.
                        StandalonePanelManager<ButtonPanel>.Panel?.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the panel width.
        /// </summary>
        public override float PanelWidth => (ButtonSize * 3f) + (ButtonSpacing * 2f);

        /// <summary>
        /// Gets the panel height.
        /// </summary>
        public override float PanelHeight => ButtonSize;

        /// <summary>
        /// Called by Unity before the first frame.
        /// Used to perform setup.
        /// </summary>
        public override void Start()
        {
            base.Start();
            UIDropDown placementDropDown = UIDropDowns.AddLabelledDropDown(this, 0f, 0f, "Placement mode");
            placementDropDown.items = _placementModeNames;
            placementDropDown.selectedIndex = 0;
            placementDropDown.eventSelectedIndexChanged += (c, index) =>
            {
                BuildingData.Instance.OverridePlacement = index != 0;
                BuildingData.Instance.Placement = (BuildingInfo.PlacementMode)(index - 1);
            };

            UIDropDown surfaceDropDown = UIDropDowns.AddLabelledDropDown(this, 0f, 30f, "Surface mode");
            surfaceDropDown.items = _surfaceModeNames;
            surfaceDropDown.selectedIndex = 0;
            surfaceDropDown.eventSelectedIndexChanged += (c, index) =>
            {
                BuildingData.Instance.SurfaceTexture = (BuildingRecord.SurfaceTexture)index;
            };

            UIDropDown terrainDropDown = UIDropDowns.AddLabelledDropDown(this, 0f, 60f, "Terrain mode");
            terrainDropDown.items = _terrainModeNames;
            terrainDropDown.selectedIndex = 0;
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
            absolutePosition = optionsBar.absolutePosition - new Vector3(0f, ButtonSize * 4f);
        }

        /// <summary>
        /// Drags the panel when the right mouse button is held down.
        /// </summary>
        /// <param name="c">Calling component (ignored).</param>
        /// <param name="p">Mouse event parameter.</param>
        private void Drag(UIComponent c, UIMouseEventParameter p)
        {
            p.Use();

            // Check for right button press.
            if ((p.buttons & UIMouseButton.Right) != 0)
            {
                // Peform dragging actions if already dragging.
                if (_dragging)
                {
                    // Calculate correct position by raycast - this is from game's UIDragHandle.
                    // Raw mouse position doesn't align with the game's UI scaling.
                    Ray ray = p.ray;
                    Vector3 inNormal = GetUIView().uiCamera.transform.TransformDirection(Vector3.back);
                    new Plane(inNormal, _lastDragPosition).Raycast(ray, out float enter);
                    Vector3 currentPosition = (ray.origin + (ray.direction * enter)).Quantize(PixelsToUnits());
                    Vector3 vectorDelta = currentPosition - _lastDragPosition;
                    Vector3[] corners = GetUIView().GetCorners();
                    Vector3 newTransformPosition = (transform.position + vectorDelta).Quantize(PixelsToUnits());

                    // Calculate panel bounds for screen constraint.
                    Vector3 upperLeft = pivot.TransformToUpperLeft(size, arbitraryPivotOffset);
                    Vector3 bottomRight = upperLeft + new Vector3(size.x, 0f - size.y);
                    upperLeft *= PixelsToUnits();
                    bottomRight *= PixelsToUnits();

                    // Constrain to screen.
                    if (newTransformPosition.x + upperLeft.x < corners[0].x)
                    {
                        newTransformPosition.x = corners[0].x - upperLeft.x;
                    }

                    if (newTransformPosition.x + bottomRight.x > corners[1].x)
                    {
                        newTransformPosition.x = corners[1].x - bottomRight.x;
                    }

                    if (newTransformPosition.y + upperLeft.y > corners[0].y)
                    {
                        newTransformPosition.y = corners[0].y - upperLeft.y;
                    }

                    if (newTransformPosition.y + bottomRight.y < corners[2].y)
                    {
                        newTransformPosition.y = corners[2].y - bottomRight.y;
                    }

                    // Apply calculated position.
                    transform.position = newTransformPosition;
                    _lastDragPosition = currentPosition;
                }
                else
                {
                    // Not already dragging, but dragging has started - commence.
                    _dragging = true;

                    // Calculate and record initial position.
                    Plane plane = new Plane(transform.TransformDirection(Vector3.back), this.transform.position);
                    Ray ray = p.ray;
                    plane.Raycast(ray, out float enter);
                    _lastDragPosition = ray.origin + (ray.direction * enter);
                }
            }
            else if (_dragging)
            {
                // We were dragging, but the mouse button is no longer held down - stop dragging.
                _dragging = false;

                // Record new position.
                StandalonePanelManager<ButtonPanel>.LastSavedXPosition = absolutePosition.x;
                StandalonePanelManager<ButtonPanel>.LastSavedYPosition = absolutePosition.y;
                ModSettings.Save();
            }
        }

        /// <summary>
        /// Adds a multi-state toggle button to the specified UIComponent.
        /// </summary>
        /// <param name="parent">Parent UIComponent.</param>
        /// <param name="name">Button name.</param>
        /// <param name="atlas">Button atlas.</param>
        /// <param name="disabledSprite">Foreground sprite for 'disabled' state..</param>
        /// <param name="enabledSprite">Foreground sprite for 'enabled' state.</param>
        /// <returns>New UIMultiStateButton.</returns>
        private UIMultiStateButton AddToggleButton(UIComponent parent, string name, UITextureAtlas atlas, string disabledSprite, string enabledSprite)
        {
            // Create button.
            UIMultiStateButton newButton = parent.AddUIComponent<UIMultiStateButton>();
            newButton.name = name;
            newButton.atlas = atlas;

            // Get sprite sets.
            UIMultiStateButton.SpriteSetState fgSpriteSetState = newButton.foregroundSprites;
            UIMultiStateButton.SpriteSetState bgSpriteSetState = newButton.backgroundSprites;

            // State 0 background.
            UIMultiStateButton.SpriteSet bgSpriteSetZero = bgSpriteSetState[0];
            if (s_transparentUI)
            {
                bgSpriteSetZero.hovered = "TransparentBaseHovered";
                bgSpriteSetZero.pressed = "TransparentBaseFocused";
            }
            else
            {
                bgSpriteSetZero.normal = "OptionBase";
                bgSpriteSetZero.focused = "OptionBase";
                bgSpriteSetZero.hovered = "OptionBaseHovered";
                bgSpriteSetZero.pressed = "OptionBasePressed";
                bgSpriteSetZero.disabled = "OptionBase";
            }

            // State 0 foreground.
            UIMultiStateButton.SpriteSet fgSpriteSetZero = fgSpriteSetState[0];
            fgSpriteSetZero.normal = disabledSprite;
            fgSpriteSetZero.focused = disabledSprite;
            fgSpriteSetZero.hovered = disabledSprite;
            fgSpriteSetZero.pressed = disabledSprite;
            fgSpriteSetZero.disabled = disabledSprite;

            // Add state 1.
            fgSpriteSetState.AddState();
            bgSpriteSetState.AddState();

            // State 1 background.
            UIMultiStateButton.SpriteSet bgSpriteSetOne = bgSpriteSetState[1];
            if (s_transparentUI)
            {
                bgSpriteSetOne.normal = "TransparentBaseFocused";
                bgSpriteSetOne.focused = "TransparentBaseFocused";
                bgSpriteSetOne.hovered = "TransparentBaseHovered";
            }
            else
            {
                bgSpriteSetOne.normal = "OptionBaseFocused";
                bgSpriteSetOne.focused = "OptionBaseFocused";
                bgSpriteSetOne.hovered = "OptionBaseHovered";
                bgSpriteSetOne.pressed = "OptionBasePressed";
                bgSpriteSetOne.disabled = "OptionBase";
            }

            // State 1 foreground.
            UIMultiStateButton.SpriteSet fgSpriteSetOne = fgSpriteSetState[1];
            fgSpriteSetOne.normal = enabledSprite;
            fgSpriteSetOne.focused = enabledSprite;
            fgSpriteSetOne.hovered = enabledSprite;
            fgSpriteSetOne.pressed = enabledSprite;
            fgSpriteSetOne.disabled = enabledSprite;

            // Set initial state.
            newButton.state = UIMultiStateButton.ButtonState.Normal;
            newButton.activeStateIndex = 0;

            // Size and appearance.
            newButton.autoSize = false;
            newButton.width = ButtonSize;
            newButton.height = ButtonSize;
            newButton.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            newButton.spritePadding = new RectOffset(0, 0, 0, 0);
            newButton.playAudioEvents = true;

            // Enforce defaults.
            newButton.canFocus = false;
            newButton.enabled = true;
            newButton.isInteractive = true;
            newButton.isVisible = true;

            return newButton;
        }
    }
}

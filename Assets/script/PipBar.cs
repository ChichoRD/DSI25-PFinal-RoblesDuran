using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A class that represents a pip bar in the UI.
/// Can adjust and modify:
///     - The number of pips displayed
///     - The number of pips "active"
///     - The icon of the active pips
///     - The icon of the inactive pips
///     - The tint of the active pips
///     - The tint of the inactive pips
/// </summary>
public class PipBar : VisualElement
{
    private uint _pips;
    private uint _activePips;
    private Texture2D _activePipImage;
    private Texture2D _inactivePipImage;
    private Color _inactivePipTint;
    private Color _activePipTint;
    private readonly List<Image> _pipsList = new List<Image>();

    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<PipBar, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlIntAttributeDescription _pipsAttribute =
            new UxmlIntAttributeDescription { name = "pips", defaultValue = 3 };
        private readonly UxmlIntAttributeDescription _activePipsAttribute =
            new UxmlIntAttributeDescription { name = "active-pips", defaultValue = 3 };
        private readonly UxmlStringAttributeDescription _activePipImageAttribute =
            new UxmlStringAttributeDescription { name = "active-pip-image-path" };
        private readonly UxmlStringAttributeDescription _inactivePipImageAttribute =
            new UxmlStringAttributeDescription { name = "inactive-pip-image-path" };
        private readonly UxmlColorAttributeDescription _activePipTintAttribute =
            new UxmlColorAttributeDescription { name = "active-pip-tint" };
        private readonly UxmlColorAttributeDescription _inactivePipTintAttribute =
            new UxmlColorAttributeDescription { name = "inactive-pip-tint" };
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
            get {
                yield break;
            }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            if (ve is PipBar pipBar)
            {
                bool valid = true;
                pipBar._activePipTint = _activePipTintAttribute.GetValueFromBag(bag, cc);
                pipBar._inactivePipTint = _inactivePipTintAttribute.GetValueFromBag(bag, cc);

                Texture2D active =
                    Resources.Load<Texture2D>(_activePipImageAttribute.GetValueFromBag(bag, cc));
                Texture2D inactive =
                    Resources.Load<Texture2D>(_inactivePipImageAttribute.GetValueFromBag(bag, cc));
                
                if (active != null) {
                    pipBar._activePipImage = active;
                } else {
                    valid = false;
                    // Debug.LogWarning("error: active pip image not found");
                }

                if (inactive != null) {
                    pipBar._inactivePipImage = inactive;
                } else {
                    valid = false;
                    // Debug.LogWarning("error: inactive pip image not found");
                }

                int pips = _pipsAttribute.GetValueFromBag(bag, cc);
                int activePips = _activePipsAttribute.GetValueFromBag(bag, cc);
                if (pips >= 0) {
                    pipBar._pips = (uint)pips;
                } else {
                    valid = false;
                    // Debug.LogWarning("error: pips may not be negative");
                }

                if (activePips <= pips) {
                    pipBar._activePips = (uint)activePips;
                } else {
                    valid = false;
                    // Debug.LogWarning("error: active pips may not be greater than total pips");
                }

                if (valid) {
                    pipBar.UpdatePipBar();
                } else {
                    // Debug.LogWarning("error: invalid pip bar configuration");
                }
            }
        }
    }

    private void UpdatePipBar()
    {
        Clear();
        _pipsList.Clear();
        for (int i = 0; i < _pips; i++)
        {
            Image pip = new Image {
                image = i < _activePips ? _activePipImage : _inactivePipImage
            };
            pip.tintColor = i < _activePips ? _activePipTint : _inactivePipTint;
            // pip.scaleMode = ScaleMode.StretchToFill;
            pip.style.flexGrow = 1.0f;
            Add(pip);
            _pipsList.Add(pip);
        }
    }

    public uint ActivePips
    {
        get => _activePips;
        set
        {
            if (value > _pips)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Active pips cannot exceed total pips.");
            } else {
                _activePips = value;
                UpdatePipBar();
            }
        }
    }
    public uint Pips
    {
        get => _pips;
        set
        {
            if (value < _activePips)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Total pips cannot be less than active pips.");
            } else {
                _pips = value;
                UpdatePipBar();
            }
        }
    }

    public PipBar()
    {
        _inactivePipTint = Color.gray;
        _activePipTint = Color.white;
        _inactivePipImage = null;
        _activePipImage = null;
        _activePips = 0;
        _pips = 0;
        _pipsList = new List<Image>();
    }
}
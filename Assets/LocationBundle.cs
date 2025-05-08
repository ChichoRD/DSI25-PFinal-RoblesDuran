using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class LocationBundle
{
    private Location _location;
    private LocationInfo _locationInfo;
    public Location Location => _location;
    public LocationInfo LocationInfo => _locationInfo;

    private VisualElement _locationPanel;
    private Label _locationNotesLabel;
    private TextField _locationNotesInput;
    private VisualElement _locationUserSelectionPanel;
    private readonly List<VisualElement> _locationUserIcons = new List<VisualElement>();
    public List<VisualElement> LocationUserIcons => _locationUserIcons;
    public LocationBundle(Location location, LocationInfo locationInfo)
    {
        _location = location;
        _locationInfo = locationInfo;

        _locationPanel = location.Root.Q<VisualElement>("location-panel");
        _locationNotesLabel = _locationPanel.Q<Label>("location-info-user-notes-label");
        _locationNotesInput = _locationPanel.Q<TextField>("location-info-user-notes-text-field");
        _locationUserSelectionPanel = _locationPanel.Q<VisualElement>("location-icon-selection-container");

        _locationNotesLabel.RegisterCallback<PointerDownEvent>(OnLocationNotesLabelPointerDown);
        _locationNotesInput.RegisterValueChangedCallback(OnLocationNotesInputValueChanged);
        locationInfo.UserIcon.RegisterCallback<PointerDownEvent>(OnLocationUserIconPointerDown);
        foreach (var icon in _locationUserSelectionPanel.Children())
        {
            _locationUserIcons.Add(icon);
            icon.RegisterCallback<ClickEvent>(OnUserIconSelected);
        }
    }

    private void OnLocationUserIconPointerDown(PointerDownEvent evt)
    {
        ShowLocationUserSelectionPanel();
    }

    public event Action<string> UserIconPathSet;
    private void OnUserIconSelected(ClickEvent evt)
    {
        if (evt.currentTarget is VisualElement icon)
        {
            string iconPath = "location/silver_" + icon.name;
            _locationInfo.Model.UserIconPath = iconPath;
            HideLocationUserSelectionPanel();
            UserIconPathSet?.Invoke(iconPath);
        }
    }

    private void OnLocationNotesInputValueChanged(ChangeEvent<string> evt)
    {
        _locationInfo.Model.UserNotes = evt.newValue;
    }

    private void OnLocationNotesLabelPointerDown(PointerDownEvent evt)
    {
        _locationNotesInput.style.visibility =
            _locationNotesInput.style.visibility == Visibility.Visible
            ? Visibility.Hidden
            : Visibility.Visible;
    }

    public void HideLocationPanel()
    {
        _locationPanel.style.visibility = Visibility.Hidden;
    }
    public void ShowLocationPanel()
    {
        _locationPanel.style.visibility = Visibility.Visible;
    }

    public void HideLocationUserSelectionPanel()
    {
        _locationUserSelectionPanel.style.visibility = Visibility.Hidden;
    }
    public void ShowLocationUserSelectionPanel()
    {
        _locationUserSelectionPanel.style.visibility = Visibility.Visible;
    }

    public void HideLocationNotesInput()
    {
        _locationNotesInput.style.visibility = Visibility.Hidden;
    }
    public void ShowLocationNotesInput()
    {
        _locationNotesInput.style.visibility = Visibility.Visible;
    }
}
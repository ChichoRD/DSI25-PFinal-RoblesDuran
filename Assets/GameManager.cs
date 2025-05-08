using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Vector2 _locationIconOffset;
    private VisualTreeAsset _locationTemplate;
    private AnnouncementMenu _announcementMenu;
    private VisualElement _mapPlayArea;

    private LocationTabsController _locationTabsController;
    private readonly List<Location> _locations = new List<Location>();

    void Awake()
    {
        _locationTabsController = GetComponent<LocationTabsController>();
    }

    void Start()
    {
        UIDocument uIDocument = GetComponent<UIDocument>();
        VisualElement root = uIDocument.rootVisualElement;
        _mapPlayArea = root.Q("map-play-area");

        _locationTemplate = Resources.Load<VisualTreeAsset>("template/location");
        _announcementMenu = new AnnouncementMenu();
        _mapPlayArea.Add(_announcementMenu);
        _announcementMenu.Hide();
        
        _mapPlayArea.RegisterCallback<PointerDownEvent>(MapPlayAreaOnPointerDown);
    }

    private void MapPlayAreaOnPointerDown(PointerDownEvent evt)
    {
        // add a new location at the clicked position
        // create a new root element for the location
        VisualElement locationRoot = _locationTemplate.CloneTree();
        locationRoot.style.position = Position.Absolute;
        locationRoot.style.left = evt.localPosition.x - _locationIconOffset.x;
        locationRoot.style.top = evt.localPosition.y - _locationIconOffset.y;
        // Debug.Log($"location root position: {locationRoot.resolvedStyle.left}, {locationRoot.resolvedStyle.top}");
        // Debug.Log($"location root size: {locationRoot.resolvedStyle.width}, {locationRoot.resolvedStyle.height}");
        // Debug.Log($"pointer position: {evt.localPosition.x}, {evt.localPosition.y}");
        _mapPlayArea.Add(locationRoot);

        // create a new location model
        LocationModel locationModel = new LocationModel(
            new LocationModel.LocationData
            {
                name = "New Location",
                description = "New Location Description",
                type = LocationModel.LocationType.Village,
            }
        );

        Location location = new Location(locationModel, locationRoot);
        _locations.Add(location);
    }
}

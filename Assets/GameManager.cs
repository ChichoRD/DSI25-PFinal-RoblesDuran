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
    private VisualElement _locationTabsRoot;    
    private LocationTabsController _locationTabsController;
    private readonly List<LocationBundle> _locations = new List<LocationBundle>();

    [Serializable]
    public struct SerializedLocations
    {
        public LocationModel.LocationData[] locations;

        public SerializedLocations(LocationModel.LocationData[] locations)
        {
            this.locations = locations;
        }
    }
    private LocationModel.LocationData[] _locationsData = Array.Empty<LocationModel.LocationData>();

    void Awake()
    {
        _locationTabsController = GetComponent<LocationTabsController>();
        TextAsset jsonLocations = Resources.Load<TextAsset>("locations");
        if (jsonLocations != null)
        {
            SerializedLocations serializedLocations = JsonUtility.FromJson<SerializedLocations>(jsonLocations.text);
            _locationsData = serializedLocations.locations;
            // shuffle the locations
            for (int i = 0; i < _locationsData.Length; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, _locationsData.Length);
                (_locationsData[randomIndex], _locationsData[i]) =
                    (_locationsData[i], _locationsData[randomIndex]);
            }
            Debug.Log($"Loaded {_locationsData.Length} locations from {jsonLocations.name}");
        }
        else
        {
            Debug.LogError("error: locations.json not found in Resources folder");
        }
    }

    void Start()
    {
        UIDocument uIDocument = GetComponent<UIDocument>();
        VisualElement root = uIDocument.rootVisualElement;
        _mapPlayArea = root.Q("map-play-area");
        _locationTabsRoot = root.Q("location-tabs-container");

        _locationTemplate = Resources.Load<VisualTreeAsset>("template/location");
        _announcementMenu = new AnnouncementMenu();
        _mapPlayArea.Add(_announcementMenu);
        _announcementMenu.Hide();
        
        _mapPlayArea.RegisterCallback<PointerDownEvent>(MapPlayAreaOnPointerDown);
    }

    private void MapPlayAreaOnPointerDown(PointerDownEvent evt)
    {
        if (_locations.Count >= _locationsData.Length)
        {
            Debug.LogWarning("error: maximum number of locations reached");
            return;
        }

        // create a new location model
        LocationModel locationModel = new LocationModel(_locationsData[
            _locations.Count
        ]);

        // add a new location at the clicked position
        // create a new root element for the location
        VisualElement locationRoot = _locationTemplate.CloneTree();
        locationRoot.style.position = Position.Absolute;
        float position_x = evt.localPosition.x - _locationIconOffset.x;
        float position_y = evt.localPosition.y - _locationIconOffset.y;

        locationRoot.style.left = position_x;
        locationRoot.style.top = position_y;
        _mapPlayArea.Add(locationRoot);

        string[] userIconPaths = {
            "location/silver_anvil",
            "location/silver_happy",
            "location/silver_sad",
            "location/silver_bow",
            "location/silver_sword",
            "location/silver_star",
            "location/silver_flag",
            "location/silver_feather",
            "location/silver_castle"
        };
        Location location = new Location(locationModel, locationRoot);
        LocationInfo locationInfo = new LocationInfo(new LocationInfoModel(new LocationInfoModel.LocationInfoData{
            position_x = position_x,
            position_y = position_y,
            userIconPath = userIconPaths[UnityEngine.Random.Range(0, userIconPaths.Length)],
            userNotes = "Click to edit notes"
        }), locationRoot);
        LocationBundle locationBundle = new LocationBundle(location, locationInfo);

        locationBundle.HideLocationNotesInput();
        locationBundle.HideLocationUserSelectionPanel();
        locationBundle.Location.LocationIcon.RegisterCallback<PointerEnterEvent>(evt => {
            locationBundle.ShowLocationPanel();
        });
        locationBundle.Location.LocationIcon.RegisterCallback<PointerLeaveEvent>(evt => {
            locationBundle.HideLocationPanel();
        });

        _locationTabsController.AddLocationTab(new LocationTabModel.LocationTabData{
            name = location.Model.Location.name,
            locationIndex = (uint)_locations.Count,
            userIconPath = locationInfo.Model.UserIconPath
        }, _locationTabsRoot);

        _locations.Add(locationBundle);
    }
}

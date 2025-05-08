using System;
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
    private LocationTab _selectedLocationTab = null;
    private PipBar _travelBar;
    private Button _passDayButton;
    private uint _currentDay = 0;

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

    [Serializable]
    public struct DailyAnnouncement
    {
        public string title;
        public string description;
        public DailyAnnouncement(string title, string description)
        {
            this.title = title;
            this.description = description;
        }
    }
    [Serializable]
    public struct SerializedAnnouncements
    {
        public DailyAnnouncement[] announcements;
        public SerializedAnnouncements(DailyAnnouncement[] announcements)
        {
            this.announcements = announcements;
        }
    }
    private DailyAnnouncement[] _dailyAnnouncements = Array.Empty<DailyAnnouncement>();

    [Serializable]
    public struct CustomLocationData
    {
        public LocationModel.LocationData locationData;
        public LocationInfoModel.LocationInfoData locationInfoData;
        public CustomLocationData(LocationModel.LocationData locationData, LocationInfoModel.LocationInfoData locationInfoData)
        {
            this.locationData = locationData;
            this.locationInfoData = locationInfoData;
        }
    }

    [Serializable]
    public struct SerializedCustomLocations
    {
        public uint currentDay;
        public uint travelPips;
        public CustomLocationData[] locations;
        public SerializedCustomLocations(uint currentDay, uint travelPips, CustomLocationData[] locations)
        {
            this.currentDay = currentDay;
            this.travelPips = travelPips;
            this.locations = locations;
        }
    }

    void Awake()
    {
        _locationTabsController = GetComponent<LocationTabsController>();
        TextAsset jsonLocations = Resources.Load<TextAsset>("locations");
        if (jsonLocations != null)
        {
            SerializedLocations serializedLocations = JsonUtility.FromJson<SerializedLocations>(jsonLocations.text);
            _locationsData = serializedLocations.locations;
            
            const int mountainRepeatCount = 6;
            LocationModel.LocationData[] modifiedLocations = new LocationModel.LocationData[_locationsData.Length + mountainRepeatCount];
            LocationModel.LocationData mountainData = default;
            for (int i = 0; i < _locationsData.Length; i++)
            {
                var data = _locationsData[i];
                if (data.type == LocationModel.LocationType.Mountain)
                {
                    mountainData = data;
                }
                modifiedLocations[i] = data;
            }
            for (int i = 0; i < mountainRepeatCount; i++)
            {
                modifiedLocations[_locationsData.Length + i] = mountainData;
            }
            _locationsData = modifiedLocations;

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

        TextAsset jsonAnnouncements = Resources.Load<TextAsset>("announcements");
        if (jsonAnnouncements != null)
        {
            SerializedAnnouncements serializedAnnouncements = JsonUtility.FromJson<SerializedAnnouncements>(jsonAnnouncements.text);
            _dailyAnnouncements = serializedAnnouncements.announcements;
            for (int i = 1; i < _dailyAnnouncements.Length; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, _dailyAnnouncements.Length);
                (_dailyAnnouncements[randomIndex], _dailyAnnouncements[i]) =
                    (_dailyAnnouncements[i], _dailyAnnouncements[randomIndex]);
            }
            Debug.Log($"Loaded {_dailyAnnouncements.Length} announcements from {jsonAnnouncements.name}");
        }
        else
        {
            Debug.LogError("error: announcements.json not found in Resources folder");
        }
    }

    void Start()
    {
        UIDocument uIDocument = GetComponent<UIDocument>();
        VisualElement root = uIDocument.rootVisualElement;
        _mapPlayArea = root.Q("map-play-area");
        _locationTabsRoot = root.Q("location-tabs-container");
        _travelBar = root.Q<PipBar>("bar-travel");
        _passDayButton = root.Q<Button>("button-sleep");

        _locationTemplate = Resources.Load<VisualTreeAsset>("template/location");
        _announcementMenu = root.Q<AnnouncementMenu>("announcement-menu");

        
        _mapPlayArea.RegisterCallback<PointerDownEvent>(MapPlayAreaOnPointerDown);
        _passDayButton.clicked += OnPassDayButtonClicked;
        if (TryLoadSavedGame()) {
            Debug.Log("Loaded saved game");
        } else {
            Debug.Log("No saved game found, starting new game");
        }

        uint remainingLocations = DayStart();
        if (remainingLocations == 0) 
        {
            OnFinalDayStart();
        }
    }

    private static string GetSaveFilePath()
    {
        return $"{Application.persistentDataPath}/save.json";
    }
    private static void SaveGame(SerializedCustomLocations customLocations)
    {
        string json = JsonUtility.ToJson(customLocations, true);
        System.IO.File.WriteAllText(GetSaveFilePath(), json);
    }
    private bool TryLoadSavedGame()
    {
        string filePath = GetSaveFilePath();
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            var customLocations = JsonUtility.FromJson<SerializedCustomLocations>(json);
            _currentDay = customLocations.currentDay;
            _travelBar.ActivePips = customLocations.travelPips;
            foreach (var locationData in customLocations.locations)
            {
                AddLocation(locationData.locationData, locationData.locationInfoData);
            }
            Debug.Log($"Loaded ${customLocations.locations.Length} locations from save file");
            return true;
        }
        return false;
    }

    private void OnPassDayButtonClicked()
    {
        ++_currentDay;

        uint remainingLocations = DayStart();
        if (remainingLocations == 0) 
        {
            OnFinalDayStart();
        }
    }

    private uint DayStart()
    {
        int remainingLocations = _locationsData.Length - _locations.Count;
        Debug.Assert(remainingLocations >= 0, "error: remaining locations should be non-negative");
        _travelBar.ActivePips = (uint)Mathf.Min((int)_travelBar.Pips, remainingLocations);

        var annnouncement = _dailyAnnouncements[_currentDay % _dailyAnnouncements.Length];
        _announcementMenu.SetAnnouncement(
            annnouncement.title,
            annnouncement.description
        );
        _announcementMenu.BringToFront();
        _announcementMenu.Show();

        return (uint)remainingLocations;
    }

    private void OnFinalDayStart()
    {
        throw new NotImplementedException();
    }

    private LocationBundle AddLocation(LocationModel.LocationData locationData, LocationInfoModel.LocationInfoData locationInfoData)
    {
        // create a new location model
        LocationModel locationModel = new LocationModel(locationData);

        // add a new location at the clicked position
        // create a new root element for the location
        VisualElement locationRoot = _locationTemplate.CloneTree();
        locationRoot.style.position = Position.Absolute;
        locationRoot.style.left = locationInfoData.position_x;
        locationRoot.style.top = locationInfoData.position_y;
        _mapPlayArea.Add(locationRoot);

        Location location = new Location(locationModel, locationRoot);
        LocationInfo locationInfo = new LocationInfo(new LocationInfoModel(new LocationInfoModel.LocationInfoData{
            position_x = locationInfoData.position_x,
            position_y = locationInfoData.position_y,
            userIconPath = locationInfoData.userIconPath,
            userNotes = locationInfoData.userNotes
        }), locationRoot);
        LocationBundle locationBundle = new LocationBundle(location, locationInfo);

        locationBundle.HideLocationNotesInput();
        locationBundle.HideLocationUserSelectionPanel();
        locationBundle.Location.LocationIcon.RegisterCallback<PointerEnterEvent>(evt => {
            locationBundle.ShowLocationPanel();
        });
        locationBundle.Location.LocationIcon.RegisterCallback<PointerLeaveEvent>(evt => {
            locationBundle.HideLocationNotesInput();
            locationBundle.HideLocationUserSelectionPanel();
            locationBundle.HideLocationPanel();
        });
        locationBundle.HideLocationPanel();
        locationBundle.LocationInfo.UserIcon.AddToClassList("clickable-border");
        locationBundle.LocationInfo.UserNotesLabel.AddToClassList("clickable-border");
        foreach (var icon in locationBundle.LocationUserIcons)
        {
            icon.AddToClassList("clickable-border");
        }

        if (location.Model.Location.type != LocationModel.LocationType.Mountain) {
            LocationTab tab = _locationTabsController.AddLocationTab(new LocationTabModel.LocationTabData{
                name = location.Model.Location.name,
                locationIndex = (uint)_locations.Count,
                userIconPath = locationInfo.Model.UserIconPath
            }, _locationTabsRoot);
            locationBundle.UserIconPathSet += iconPath =>
            {
                tab.Model.UserIconPath = iconPath;
            };
            tab.Root.AddToClassList("location-tab");
            tab.Root.RegisterCallback<PointerDownEvent>(evt =>
            {
                OnLocationTabSelected(evt, tab);
            });
        }

        _locations.Add(locationBundle);
        return locationBundle;
    }

    private void MapPlayAreaOnPointerDown(PointerDownEvent evt)
    {   
        if (_travelBar.ActivePips == 0) {
            Debug.LogWarning("error: no travel pips available");
            return;
        }
        else if (_locations.Count >= _locationsData.Length)
        {
            Debug.LogWarning("error: maximum number of locations reached");
            return;
        }

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
        string userIconPath = userIconPaths[UnityEngine.Random.Range(0, userIconPaths.Length)];
        LocationModel.LocationData locationData = _locationsData[_locations.Count];
        LocationInfoModel.LocationInfoData locationInfoData = new LocationInfoModel.LocationInfoData
        {
            position_x = evt.localPosition.x - _locationIconOffset.x,
            position_y = evt.localPosition.y - _locationIconOffset.y,
            userIconPath = userIconPath,
            userNotes = "Click to edit notes"
        };
        LocationBundle locationBundle = AddLocation(locationData, locationInfoData);
        --_travelBar.ActivePips;
    }

    private void OnLocationTabSelected(PointerDownEvent evt, LocationTab tab)
    {
        if (_selectedLocationTab != null)
        {
            _selectedLocationTab.Root.RemoveFromClassList("location-tab-selected");
            _locations[(int)_selectedLocationTab.Model.LocationIndex].HideLocationPanel();
        }
        _selectedLocationTab = tab;
        tab.Root.AddToClassList("location-tab-selected");
        _locations[(int)_selectedLocationTab.Model.LocationIndex].ShowLocationPanel();
    }

    void OnDestroy()
    {
        SerializedCustomLocations customLocations = new SerializedCustomLocations(
            _currentDay,
            _travelBar.ActivePips,
            new CustomLocationData[_locations.Count]
        );
        for (int i = 0; i < _locations.Count; i++)
        {
            var location = _locations[i];
            customLocations.locations[i] = new CustomLocationData(
                location.Location.Model.Location,
                location.LocationInfo.Model.LocationInfo
            );
        }
        SaveGame(customLocations);
    }
}

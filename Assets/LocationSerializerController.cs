using System;
using UnityEngine;

public class LocationSerializerController : MonoBehaviour
{
    [Serializable]
    public struct LocationData
    {
        public string name;
        [TextArea]
        public string description;
        public LocationModel.LocationType type;

        public static LocationModel.LocationData Into(LocationData data)
        {
            return new LocationModel.LocationData(data.name, data.description, data.type);
        }
    }
    [Serializable]
    public struct SerializedLocations
    {
        public LocationData[] locations;

        public SerializedLocations(LocationData[] locations)
        {
            this.locations = locations;
        }
    }

    [SerializeField]
    private LocationData[] _locations = Array.Empty<LocationData>();

    public static string GetSaveDirectory()
    {
        return System.IO.Path.Combine(Application.persistentDataPath, "locations.json");
    }

    [ContextMenu(nameof(SaveLocations))]
    public void SaveLocations()
    {
        SerializedLocations serializedLocations = new SerializedLocations(_locations);
        string json = JsonUtility.ToJson(serializedLocations, true);
        System.IO.File.WriteAllText(GetSaveDirectory(), json);
        Debug.Log($"Saved locations to {GetSaveDirectory()}");
    }
    [ContextMenu(nameof(LoadLocations))]
    public void LoadLocations()
    {
        if (System.IO.File.Exists(GetSaveDirectory()))
        {
            string json = System.IO.File.ReadAllText(GetSaveDirectory());
            SerializedLocations serializedLocations = JsonUtility.FromJson<SerializedLocations>(json);
            _locations = serializedLocations.locations;
            Debug.Log($"Loaded locations from {GetSaveDirectory()}");
        }
        else
        {
            Debug.LogError($"error: file not found at {GetSaveDirectory()}");
        }
    }
}
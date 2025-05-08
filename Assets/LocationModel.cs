using System;

public class LocationModel
{
    [Serializable]
    public enum LocationType
    {
        None,
        Village,
        Town,
        City,
        Mountain,
        TreasureMountain,
        BanditStash
    }

    [Serializable]
    public struct LocationData
    {
        public string name;
        public string description;
        public LocationType type;

        public LocationData(string name, string description, LocationType type)
        {
            this.name = name;
            this.description = description;
            this.type = type;
        }
    }

    private LocationData _location;
    public Action<LocationData> LocationSet;
    public LocationData Location
    {
        get { return _location; }
        set {
            _location = value;
            LocationSet?.Invoke(value);
        }
    }

    public LocationModel(LocationData location)
    {
        Location = location;
    }
}
using System;

public class LocationTabModel
{
    [Serializable]
    public struct LocationTabData
    {
        public string name;
        public string userIconPath;
        public uint locationIndex;

        public LocationTabData(string name, string userIconPath, uint locationIndex)
        {
            this.name = name;
            this.userIconPath = userIconPath;
            this.locationIndex = locationIndex;
        }
    }
    private LocationTabData _locationTabData;

    public event Func<string, bool> NameSet;
    public event Func<string, bool> UserIconPathSet;
    public event Func<uint, bool> LocationIndexSet;
    public string Name
    {
        get => _locationTabData.name;
        set {
            if (NameSet == null || NameSet.Invoke(value) ) {
                _locationTabData.name = value;
            }
        }
    }
    public string UserIconPath
    {
        get => _locationTabData.userIconPath;
        set {
            if (UserIconPathSet == null || UserIconPathSet.Invoke(value) ) {
                _locationTabData.userIconPath = value;
            }
        }
    }
    public uint LocationIndex
    {
        get => _locationTabData.locationIndex;
        set {
            if (LocationIndexSet == null || LocationIndexSet.Invoke(value) ) {
                _locationTabData.locationIndex = value;
            }
        }
    }
    public LocationTabModel(LocationTabData locationTabData)
    {
        _locationTabData = locationTabData;
        NameSet?.Invoke(_locationTabData.name);
        UserIconPathSet?.Invoke(_locationTabData.userIconPath);
        LocationIndexSet?.Invoke(_locationTabData.locationIndex);
    }
}

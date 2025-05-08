using System;
using UnityEngine;

public class LocationInfoModel
{
    [Serializable]
    public struct LocationInfoData
    {
        public string userNotes;
        public string userIconPath;
        public float position_x;
        public float position_y;

        public LocationInfoData(string userNotes, string userIconPath, float position_x, float position_y)
        {
            this.userNotes = userNotes;
            this.userIconPath = userIconPath;
            this.position_x = position_x;
            this.position_y = position_y;
        }
    }

    private LocationInfoData _locationInfo;
    public LocationInfoData LocationInfo => _locationInfo;
    public Action<string> UserNotesSet;
    public Func<string, bool> UserIconPathSet;
    public Action<Vector2> PositionSet;

    public string UserNotes
    {
        get => _locationInfo.userNotes;
        set
        {
            _locationInfo.userNotes = value;
            UserNotesSet?.Invoke(value);
        }
    }
    public string UserIconPath
    {
        get => _locationInfo.userIconPath;
        set
        {
            if (UserIconPathSet == null || UserIconPathSet.Invoke(value))
            {
                _locationInfo.userIconPath = value;
            }
        }
    }
    public Vector2 Position
    {
        get => new Vector2(_locationInfo.position_x, _locationInfo.position_y);
        set
        {
            _locationInfo.position_x = value.x;
            _locationInfo.position_y = value.y;
            PositionSet?.Invoke(value);
        }
    }

    public LocationInfoModel(LocationInfoData locationInfo)
    {
        _locationInfo = locationInfo;
        UserNotesSet?.Invoke(_locationInfo.userNotes);
        UserIconPathSet?.Invoke(_locationInfo.userIconPath);
        PositionSet?.Invoke(new Vector2(_locationInfo.position_x, _locationInfo.position_y));
    }
}

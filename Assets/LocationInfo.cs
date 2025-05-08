using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LocationInfo
{
    private LocationInfoModel _model;
    private VisualElement _root;
    public LocationInfoModel Model => _model;
    public VisualElement Root => _root;

    
    private readonly Label _userNotesLabel;
    private readonly Image _userIcon;


    public LocationInfo(LocationInfoModel model, VisualElement root)
    {
        _model = model;
        _root = root;
        _userNotesLabel = root.Q<Label>("location-info-user-notes-label");
        _userIcon = root.Q<Image>("location-info-user-icon");

        _model.UserNotesSet += OnUserNotesSet;
        _model.UserIconPathSet += OnUserIconPathSet;
        _model.PositionSet += OnPositionSet;
        OnUserNotesSet(_model.UserNotes);
        OnUserIconPathSet(_model.UserIconPath);
    }

    private void OnPositionSet(Vector2 vector)
    {
        _root.transform.position = new Vector2(vector.x, vector.y);
    }

    private bool OnUserIconPathSet(string arg)
    {
        var image = Resources.Load<Texture2D>(arg);
        if (image != null) {
            _userIcon.image = image;
            return true;
        } else {
            Debug.LogWarning($"error: user icon not found for path: {arg}");
            return false;
        }
    }

    private void OnUserNotesSet(string obj)
    {
        _userNotesLabel.text = obj;
    }
}
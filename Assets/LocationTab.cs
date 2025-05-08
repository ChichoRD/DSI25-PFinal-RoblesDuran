using UnityEngine.UIElements;
using UnityEngine;

public class LocationTab
{
    private readonly LocationTabModel _model;
    private readonly VisualElement _root;
    public LocationTabModel Model => _model;
    public VisualElement Root => _root;


    private readonly Label _nameLabel;
    private readonly VisualElement _userIconImage;

    public LocationTab(LocationTabModel model, VisualElement root)
    {
        _model = model;
        _root = root;
        _nameLabel = root.Q<Label>("location-tab-name-label");
        _userIconImage = root.Q<VisualElement>("location-tab-user-icon-image");

        _model.NameSet += OnNameSet;
        _model.UserIconPathSet += OnUserIconPathSet;
        _model.LocationIndexSet += OnLocationIndexSet;

        OnNameSet(_model.Name);
        OnUserIconPathSet(_model.UserIconPath);
        OnLocationIndexSet(_model.LocationIndex);
    }
    private bool OnNameSet(string name)
    {
        _nameLabel.text = name;
        return true;
    }
    private bool OnUserIconPathSet(string userIconPath)
    {
        Texture2D texture = Resources.Load<Texture2D>(userIconPath);
        if (texture != null)
        {
            _userIconImage.style.backgroundImage = new StyleBackground(texture);
            return true;
        }
        else
        {
            Debug.LogWarning($"error: user icon not found at path: {userIconPath}");
            return false;
        }
    }
    private bool OnLocationIndexSet(uint locationIndex)
    {
        return true;
    }
}
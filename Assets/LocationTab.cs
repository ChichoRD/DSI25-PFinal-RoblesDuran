using UnityEngine.UIElements;
using UnityEngine;

class LocationTab
{
    private readonly LocationTabModel _model;
    private readonly VisualElement _root;
    public LocationTabModel Model => _model;
    public VisualElement Root => _root;


    private readonly Label _nameLabel;
    private readonly Image _userIconImage;

    public LocationTab(LocationTabModel model, VisualElement root)
    {
        _model = model;
        _root = root;
        _nameLabel = root.Q<Label>("location-tab-name-label");
        _userIconImage = root.Q<Image>("location-tab-user-icon-image");

        _model.NameSet += OnNameSet;
        _model.UserIconPathSet += OnUserIconPathSet;
        _model.LocationIndexSet += OnLocationIndexSet;

        UpdateUI();
    }

    private void UpdateUI()
    {
        _nameLabel.text = _model.Name;
        _userIconImage.image = Resources.Load<Texture2D>(_model.UserIconPath);
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
            _userIconImage.image = texture;
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
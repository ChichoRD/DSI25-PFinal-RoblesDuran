using UnityEngine.UIElements;
using UnityEngine;

public class Location
{
    private LocationModel _model;
    private VisualElement _root;
    public LocationModel Model => _model;
    public VisualElement Root => _root;


    private readonly VisualElement _locationIcon;
    private readonly Label _nameLabel;
    private readonly Label _descriptionLabel;


    public Location(LocationModel model, VisualElement root)
    {
        _model = model;
        _root = root;
        _locationIcon = root.Q<VisualElement>("location-icon");
        _nameLabel = root.Q<Label>("location-name-label");
        _descriptionLabel = root.Q<Label>("location-description-label");

        _model.LocationSet += OnLocationSet;
        OnLocationSet(_model.Location);
    }

    private void OnLocationSet(LocationModel.LocationData data)
    {
        _nameLabel.text = data.name;
        _descriptionLabel.text = data.description;

        var image = data.type switch
        {
            LocationModel.LocationType.None => null,
            LocationModel.LocationType.Village => Resources.Load<Texture2D>("location/village"),
            LocationModel.LocationType.Town => Resources.Load<Texture2D>("location/town"),
            LocationModel.LocationType.City => Resources.Load<Texture2D>("location/city"),
            LocationModel.LocationType.Mountain => Resources.Load<Texture2D>("location/mountain"),
            LocationModel.LocationType.TreasureMountain => Resources.Load<Texture2D>("location/treasure-mountain"),
            LocationModel.LocationType.BanditStash => Resources.Load<Texture2D>("location/bandit-stash"),
            _ => null,
        };

        if (image != null) {
            _locationIcon.style.backgroundImage = new StyleBackground(image);
        } else {
            Debug.LogWarning($"error: location icon not found for type: {data.type}");
        }
    }
}

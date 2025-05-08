using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class LocationTabsController : MonoBehaviour
{
    [Serializable]
    public struct SerializedLocationTabs {
        public LocationTabModel.LocationTabData[] locationTabs;

        public SerializedLocationTabs(LocationTabModel.LocationTabData[] locationTabs) {
            this.locationTabs = locationTabs;
        }
    }

    private readonly List<LocationTab> _locationTabs = new List<LocationTab>();
    private static string GetSaveDirectory() {
        return Path.Combine(Application.persistentDataPath, "location-tabs.json");
    }
    void Start()
    {
        const string rootName = "location-tabs-container";
        UIDocument uIDocument = GetComponent<UIDocument>();
        VisualElement root = uIDocument.rootVisualElement.Q(rootName);
        if (root == null) {
            Debug.LogError($"error: root element with name {rootName} not found");
            return;
        }


        if (File.Exists(GetSaveDirectory())) {
            string json = File.ReadAllText(GetSaveDirectory());
            SerializedLocationTabs serializedLocationTabs = JsonUtility.FromJson<SerializedLocationTabs>(json);
            foreach (var locationTabData in serializedLocationTabs.locationTabs) {
                LocationTab locationTab = AddLocationTab(locationTabData, root);
                if (locationTab != null) {
                    _locationTabs.Add(locationTab);
                } else {
                    Debug.LogError($"error: failed to add location tab for {locationTabData.name}");
                }
            }
        }
    }

    public LocationTab AddLocationTab(LocationTabModel.LocationTabData locationTabData, VisualElement root) {
        const string templatePath = "template/location-tab";
        var template = Resources.Load<VisualTreeAsset>(templatePath);
        if (template == null) {
            Debug.LogError($"error: template not found");
            return null;
        }
        VisualElement locationTabRoot = template.CloneTree();
        LocationTab locationTab = new LocationTab(new LocationTabModel(locationTabData), locationTabRoot);
        root.Add(locationTabRoot);
        return locationTab;
    }

    private uint SaveLocationTabs() {
        SerializedLocationTabs serializedLocationTabs = new SerializedLocationTabs(new LocationTabModel.LocationTabData[_locationTabs.Count]);
        for (int i = 0; i < _locationTabs.Count; i++) {
            serializedLocationTabs.locationTabs[i] = new LocationTabModel.LocationTabData(
                _locationTabs[i].Model.Name,
                _locationTabs[i].Model.UserIconPath,
                _locationTabs[i].Model.LocationIndex
            );
        }
        string json = JsonUtility.ToJson(serializedLocationTabs, true);
        File.WriteAllText(GetSaveDirectory(), json);
        return (uint)_locationTabs.Count;
    }
    private void OnDestroy() {
        SaveLocationTabs();
    }
}

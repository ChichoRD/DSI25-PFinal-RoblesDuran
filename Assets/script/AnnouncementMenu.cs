using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnnouncementMenu : VisualElement
{
    private string _title;
    private string _announcement;
    private Button _okButton;
    private Label _titleLabel;
    private Label _announcementLabel;

    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<AnnouncementMenu, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription _titleAttribute =
            new UxmlStringAttributeDescription { name = "title" };
        private readonly UxmlStringAttributeDescription _announcementAttribute =
            new UxmlStringAttributeDescription { name = "announcement" };
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get
            {
                yield return new UxmlChildElementDescription(typeof(Label));
                yield return new UxmlChildElementDescription(typeof(Label));
                yield return new UxmlChildElementDescription(typeof(Button));
            }
        }
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            if (ve is AnnouncementMenu menu)
            {
                string title = _titleAttribute.GetValueFromBag(bag, cc);
                string announcement = _announcementAttribute.GetValueFromBag(bag, cc);
                menu.SetAnnouncement(title, announcement);
            }
        }
    }

    private const string templatePath = "template/announcement-menu";
    private const string titleName = "announcement-menu-title";
    private const string announcementName = "announcement-menu-text";
    private const string okButtonName = "announcement-menu-ok-button";

    public AnnouncementMenu()
    {
        var template = Resources.Load<VisualTreeAsset>(templatePath);
        template.CloneTree(this);

        _titleLabel =           this.Q<Label>(titleName);
        _announcementLabel =    this.Q<Label>(announcementName);
        _okButton =             this.Q<Button>(okButtonName);

        _titleLabel.text = _title;
        _announcementLabel.text = _announcement;

        _okButton.clickable.clicked += OnOkButtonClicked;
    }

    public void SetAnnouncement(string title, string announcement)
    {
        _title = title;
        _announcement = announcement;

        _titleLabel.text = _title;
        _announcementLabel.text = _announcement;
    }

    public void Show()
    {
        this.style.display = DisplayStyle.Flex;
    }
    public void Hide()
    {
        this.style.display = DisplayStyle.None;
    }

    private void OnOkButtonClicked()
    {
        Hide();
    }
}

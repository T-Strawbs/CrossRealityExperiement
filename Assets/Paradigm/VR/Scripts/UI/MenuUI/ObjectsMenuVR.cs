using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsMenuVR : MenuUI
{
    public override string MenuName { get; protected set; } = "Object MenuUI";

    [SerializeField] private Button _backButton;

    [SerializeField] private RectTransform _objectOptionContainer;
    [SerializeField] private ObjectOption _objectOptionPrefab;

    public override void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    public override void Initialise(MenuState menustate)
    {
        _menuState = menustate;
        //get the  object prefabs list from the object manager
        List<InteractableObject> objectPrefabs = ObjectManager.Instance.InteractableObjectPrefabs;

        //foreach prefab in the object prefabs list
        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            //get the InteractableObject at the current index
            InteractableObject interactableObject = objectPrefabs[i];
            //create an ObjectOption UI element and make it a child of the objectOption container
            ObjectOption objectOption = Instantiate(_objectOptionPrefab, _objectOptionContainer);
            //initialise the object option using the interactable object's name and index
            objectOption.Initialise(interactableObject.name, i);
        }
    }

    private void Awake()
    {
        _backButton.onClick.AddListener(() => Back());
    }
    private void Back()
    {
        _menuState.ChangeMenu(MenuStateEnum.MAIN);
    }
}
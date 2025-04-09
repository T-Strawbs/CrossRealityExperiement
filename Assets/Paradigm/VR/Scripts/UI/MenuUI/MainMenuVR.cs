using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuVR : Menu
{
    public override string MenuName { get; protected set; } = "Main Menu";

    [SerializeField] private Button _objectsMenuBtn;
    [SerializeField] private Button _networkMenuBtn;

    public override void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    public override void Initialise(MenuState handMenu)
    {
        _handMenu = handMenu;
        _objectsMenuBtn.onClick.AddListener(() => _handMenu.ChangeMenu(MenuStateEnum.OBJECT));
        _networkMenuBtn.onClick.AddListener(() => _handMenu.ChangeMenu(MenuStateEnum.NETWORK));
    }
    
}
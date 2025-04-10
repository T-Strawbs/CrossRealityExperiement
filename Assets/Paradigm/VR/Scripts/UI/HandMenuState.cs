using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MenuStateEnum
{
    MAIN,
    OBJECT,
    NETWORK,
    DEBUG,
    NONE
}

public class HandMenuState : MenuState
{
    [SerializeField] private TMP_Text _menuNameText;

    [SerializeField] private MenuUI _mainMenu;
    [SerializeField] private MenuUI _objectsMenu;
    [SerializeField] private MenuUI _networkMenu;

    private MenuUI _currentMenu;

    private void Awake()
    {
        InitialiseMenus();
        _currentMenu = _mainMenu;
    }

    protected override void InitialiseMenus()
    {
        _mainMenu.Initialise(this);
        _objectsMenu.Initialise(this);
        _networkMenu.Initialise(this);

        _mainMenu.gameObject.SetActive(true);
        _objectsMenu.gameObject.SetActive(false);
        _networkMenu.gameObject.SetActive(false);
    }

    public override void ChangeMenu(MenuStateEnum menuState)
    {
        //Deactivate the current menu
        _currentMenu.Deactivate();
        //check the given menu state and change the current menu accordingly
        switch (menuState)
        {
            case MenuStateEnum.MAIN:
                _currentMenu = _mainMenu;
                break;
            case MenuStateEnum.OBJECT:
                _currentMenu = _objectsMenu;
                break;
            case MenuStateEnum.NETWORK:
                _currentMenu = _networkMenu;
                break;
            default:
                break;
        }
        //activate the current menu
        _currentMenu.Activate();
        //Change the name of the current menu text
        _menuNameText.text = _currentMenu.MenuName;
    }


}

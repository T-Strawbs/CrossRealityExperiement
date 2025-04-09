using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ARAVMenuState : MenuState
{
    [SerializeField] private ToggleButton _objectMenuToggle;
    [SerializeField] private ToggleButton _networkMenuToggle;

    [SerializeField] private Menu _networkMenu;
    [SerializeField] private Menu _ObjectMenu;
    [SerializeField] private RectTransform _menuBody;

    [SerializeField] private TMP_Text _menuTitleText;

    public override void ChangeMenu(MenuStateEnum menuState)
    {
        switch(menuState)
        {
            case MenuStateEnum.NETWORK:
                //activate the menu body if inactive
                _menuBody.gameObject.SetActive(true);
                //activate the NetworkMenu
                _networkMenu.Activate();
                //set the menu title
                _menuTitleText.text = _networkMenu.MenuName;
                //deactivate the objectMenu
                _ObjectMenu.Deactivate();
                break;
            case MenuStateEnum.OBJECT:
                //activate the menu body if inactive
                _menuBody.gameObject.SetActive(true);
                //activate the ObjectMenu
                _ObjectMenu.Activate();
                //set the menu title
                _menuTitleText.text = _ObjectMenu.MenuName;
                //deactivate the NetworkMenu
                _networkMenu.Deactivate();
                break;
            default:
                //Deactivate the menu body
                _menuBody.gameObject.SetActive(false);
                //Deactivate both menus
                _networkMenu.Deactivate();
                _ObjectMenu.Deactivate();
                break;
        }
    }

    protected override void InitialiseMenus()
    {
        _networkMenu.Initialise(this);
        _ObjectMenu.Initialise(this);
    }

    private void Awake()
    {
        InitialiseMenus();
        _networkMenuToggle.onValueChanged += (toggle, state)  => ToggleButton(toggle, state);
        _objectMenuToggle.onValueChanged += (toggle, state) => ToggleButton(toggle, state);
    }

    private void ToggleButton(ToggleButton changedToggle, ToggleState toggleState)
    {
        //check if it was the network menu toggle which value changed
        if(changedToggle == _networkMenuToggle)
        {
            //check if the toggle is toggled off
            if (toggleState == ToggleState.OFF)
            {
                ChangeMenu(MenuStateEnum.NONE);
                return;
            }
            //untoggle the other toggle
            _objectMenuToggle.TurnOff();
            //change the menu 
            ChangeMenu(MenuStateEnum.NETWORK);
        }
        //else it was the object menu toggle
        else
        {
            //check if the toggle is toggled off
            if (toggleState == ToggleState.OFF)
            {
                ChangeMenu(MenuStateEnum.NONE);
                return;
            }
            //untoggle the other toggle
            _networkMenuToggle.TurnOff();
            //change the menu 
            ChangeMenu(MenuStateEnum.OBJECT);
        }
    }
}

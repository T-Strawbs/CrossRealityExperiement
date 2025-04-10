using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ARAVMenuState : MenuState
{
    [SerializeField] private ToggleButton _objectMenuToggle;
    [SerializeField] private ToggleButton _networkMenuToggle;
    [SerializeField] private ToggleButton _debugMenuToggle;

    [SerializeField] private RectTransform _menuBody;
    [SerializeField] private MenuUI _networkMenu;
    [SerializeField] private MenuUI _objectMenu;
    [SerializeField] private MenuUI _debugMenu;

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
                //deactivate the other menus
                _objectMenu.Deactivate();
                _debugMenu.Deactivate();
                break;
            case MenuStateEnum.OBJECT:
                //activate the menu body if inactive
                _menuBody.gameObject.SetActive(true);
                //activate the ObjectMenu
                _objectMenu.Activate();
                //set the menu title
                _menuTitleText.text = _objectMenu.MenuName;
                //deactivate the other menus
                _networkMenu.Deactivate();
                _debugMenu.Deactivate();
                break;
            case MenuStateEnum.DEBUG:
                //activate the menu body if inactive
                _menuBody.gameObject.SetActive(true);
                //activate the DebugMenu
                _debugMenu.Activate();
                //set the menu title
                _menuTitleText.text = _debugMenu.MenuName;
                //deactivate the other menus
                _networkMenu.Deactivate();
                _objectMenu.Deactivate();
                break;
            default:
                //Deactivate the menu body
                _menuBody.gameObject.SetActive(false);
                //Deactivate both menus
                _networkMenu.Deactivate();
                _objectMenu.Deactivate();
                _debugMenu.Deactivate();
                break;
        }
    }

    protected override void InitialiseMenus()
    {
        _networkMenu.Initialise(this);
        _objectMenu.Initialise(this);
        _debugMenu.Initialise(this);
    }

    private void Awake()
    {
        InitialiseMenus();
        _networkMenuToggle.onValueChanged += (toggle, state)  => ToggleButton(toggle, state);
        _objectMenuToggle.onValueChanged += (toggle, state) => ToggleButton(toggle, state);
        _debugMenuToggle.onValueChanged += (toggle, state) => ToggleButton(toggle, state);

        _menuBody.gameObject.SetActive(false);
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
            //untoggle the other toggles
            _objectMenuToggle.TurnOff();
            _debugMenuToggle.TurnOff();
            //change the menu 
            ChangeMenu(MenuStateEnum.NETWORK);
        }
        else if(changedToggle == _objectMenuToggle)
        {
            //check if the toggle is toggled off
            if (toggleState == ToggleState.OFF)
            {
                ChangeMenu(MenuStateEnum.NONE);
                return;
            }
            //untoggle the other toggles
            _networkMenuToggle.TurnOff();
            _debugMenuToggle.TurnOff();
            //change the menu 
            ChangeMenu(MenuStateEnum.OBJECT);
        }
        //else it was the object menu toggle
        else if(changedToggle == _debugMenuToggle)
        {
            //check if the toggle is toggled off
            if (toggleState == ToggleState.OFF)
            {
                ChangeMenu(MenuStateEnum.NONE);
                return;
            }
            //untoggle the other toggle
            _objectMenuToggle.TurnOff();
            _networkMenuToggle.TurnOff();
            //change the menu 
            ChangeMenu(MenuStateEnum.DEBUG);
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsMenuVR : MenuUI
{
    public override string MenuName { get; protected set; } = "Object MenuUI";

    [SerializeField] private Button _backButton;

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
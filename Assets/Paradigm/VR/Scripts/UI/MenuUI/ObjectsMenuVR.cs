using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsMenuVR : Menu
{
    public override string MenuName { get; protected set; } = "Object Menu";

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
        _handMenu = menustate;
    }

    private void Awake()
    {
        _backButton.onClick.AddListener(() => Back());
    }
    private void Back()
    {
        _handMenu.ChangeMenu(MenuStateEnum.MAIN);
    }
}
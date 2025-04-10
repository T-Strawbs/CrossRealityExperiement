using System.Collections;
using UnityEngine;

public abstract class MenuUI : MonoBehaviour
{
    public abstract string MenuName { get; protected set; }
    protected MenuState _menuState;

    public abstract void Initialise(MenuState menuState);
    public abstract void Activate();
    public abstract void Deactivate();

}
using System.Collections;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    public abstract string MenuName { get; protected set; }
    protected MenuState _handMenu;

    public abstract void Initialise(MenuState menuState);
    public abstract void Activate();
    public abstract void Deactivate();

}
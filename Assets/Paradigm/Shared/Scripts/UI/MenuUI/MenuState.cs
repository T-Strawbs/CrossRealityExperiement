using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class MenuState : MonoBehaviour
{
    public abstract void ChangeMenu(MenuStateEnum menuState);
    protected abstract void InitialiseMenus();

}
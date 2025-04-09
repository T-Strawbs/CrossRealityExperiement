using System.Collections;
using UnityEngine;

/// <summary>
/// Abstract class for creating polymorphic ConnectionElements
/// </summary>
public abstract class ConnectionElement : MonoBehaviour
{
    /// <summary>
    /// The reference to the NetworkMenuARAV
    /// </summary>
    protected NetworkMenu _networkMenu;
    /// <summary>
    /// Method for initialising the ConnectionElement
    /// </summary>
    /// <param name="networkMenu"></param>
    public abstract void Initialise(NetworkMenu networkMenu);
    /// <summary>
    /// Method for handling the activation of this ConnnectionElement
    /// </summary>
    public abstract void Activate();
    /// <summary>
    /// Method for handling the deactivation of this ConnnectionElement
    /// </summary>
    public abstract void Deactivate();
}
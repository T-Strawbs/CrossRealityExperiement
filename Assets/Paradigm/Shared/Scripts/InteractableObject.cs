using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{

    public void OnSelectEnter()
    {
        Debug.Log($"object: {gameObject.name} is selected");
    }

    public void OnSelectExit()
    {
        Debug.Log($"object: {gameObject.name} was unselected");
    }

    public void OnHoverEnter()
    {
        Debug.Log($"object: {gameObject.name} is hovered");
    }
    public void OnHoverExit()
    {
        Debug.Log($"object: {gameObject.name} was unhovered");
    }
}

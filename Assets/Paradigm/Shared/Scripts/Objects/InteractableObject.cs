using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InteractableObject : MonoBehaviour, RoomObject
{
    [SerializeField] private XRBaseInteractable _interactable;

    public string Name { get { return gameObject.name; } }

    private void Awake()
    {
        //subscribe to the interaction events
        _interactable.selectEntered.AddListener((args) => OnSelectEnter(args));
        _interactable.selectExited.AddListener((args) => OnSelectExit(args));
        _interactable.hoverEntered.AddListener((args) => OnHoverEnter(args));
        _interactable.hoverExited.AddListener((args) => OnHoverExit(args));
    }
    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log($"object: {gameObject.name} is selected");
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log($"object: {gameObject.name} was unselected");
    }

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        Debug.Log($"object: {gameObject.name} is hovered");
    }
    public void OnHoverExit(HoverExitEventArgs args)
    {
        Debug.Log($"object: {gameObject.name} was unhovered");
    }

    public void HandleOutOfBounds(Vector3 lastPosition)
    {
        Debug.Log($"object: {gameObject.name}'s last position was {lastPosition.ToString()} " +
            $"and its current position is {transform.position}");
        this.transform.position = lastPosition;
        Debug.Log($"object: {gameObject.name}'s new position is: {transform.position}");
        
    }
}

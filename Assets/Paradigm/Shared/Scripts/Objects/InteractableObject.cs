using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InteractableObject : MonoBehaviour, RoomObject
{
    [SerializeField] private XRBaseInteractable _interactable;

    public static int InstanceCount { get; private set; } = 0;

    public string ID { get; private set; } 
    public string Name { get { return gameObject.name; } }

    private void Awake()
    {
        InstanceCount++;

        ID = $"{name}#{InstanceCount}";

        //subscribe to the interaction events
        _interactable.selectEntered.AddListener((args) => OnSelectEnter(args));
        _interactable.selectExited.AddListener((args) => OnSelectExit(args));
        _interactable.hoverEntered.AddListener((args) => OnHoverEnter(args));
        _interactable.hoverExited.AddListener((args) => OnHoverExit(args));
    }
    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log($"object: {gameObject.name} is selected");

        //check if we are online
        if(ConnectionManager.IsOnline)
        {
            //request ownership
            ObjectManager.Instance.RequestChangeOwnershipOfObject(this,false);
        }
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log($"object: {gameObject.name} was unselected");
        //check if we are online
        if (ConnectionManager.IsOnline)
        {
            //revoke ownership
            ObjectManager.Instance.RequestChangeOwnershipOfObject(this, true);
        }
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

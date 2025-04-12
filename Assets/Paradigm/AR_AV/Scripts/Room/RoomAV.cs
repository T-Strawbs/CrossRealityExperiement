using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class for Managing the Entites in the Room Object.
/// </summary>
public class RoomAV : MonoBehaviour
{

    /// <summary>
    /// The player proxy objects that have entered the room at least once
    /// </summary>
    private Dictionary<RoomObject,Vector3> _roomObjects = new Dictionary<RoomObject, Vector3>();
    /// <summary>
    /// A Stack to hold reference to the player proxies that have traveled out of the room
    /// </summary>
    private Stack<RoomObject> _outOfBoundsObjects = new Stack<RoomObject>();

    private void OnTriggerStay(Collider collider)
    {
        //check if the collision object is is an RoomObject
        RoomObject roomObject = collider.GetComponent<RoomObject>();
        //if its not return 
        if (roomObject == null)
            return;
        //check if this RoomObject was already known of
        else if (!_roomObjects.ContainsKey(roomObject))
        {
            //add it as it wasnt recognised before
            _roomObjects.Add(roomObject, Vector3.zero);
            //Debug.Log($"{proxy.gameObject.name} was discovered");
        }
        //store its last known position
        _roomObjects[roomObject] = collider.transform.position;
        //Debug.Log($"{proxy.gameObject.name};s last known position was {_roomObjects[proxy].ToString()}");

    }
    private void OnTriggerExit(Collider collider)
    {
        //check if the exiting colider is a RoomObject
        RoomObject roomObject = collider.GetComponent<RoomObject>();
        //if its not then return
        if(roomObject == null)
            return;

        //tell the RoomObject to return to its last position in the room
        _outOfBoundsObjects.Push(roomObject);
        Debug.Log($"{roomObject.Name} left the room");
    }

    private void LateUpdate()
    {
        //check if theres any player RoomObjects that are out of bounds
        if (_outOfBoundsObjects.Count < 1)
            //there isnt so lets return
            return;
        //while the out of bounds stack isnt empty
        while(_outOfBoundsObjects.Count > 0)
        {
            //get the next out of bounds RoomObject
            RoomObject roomObject = _outOfBoundsObjects.Pop();
            //get its last postion
            Vector3 lastKnownPosition = _roomObjects[roomObject];
            roomObject.HandleOutOfBounds(lastKnownPosition);
            Debug.Log($"{roomObject.Name} returned to the room");
        }
    }

}

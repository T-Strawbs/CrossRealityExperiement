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
    private Dictionary<PlayerProxy,Vector3> _knownPlayerProxies = new Dictionary<PlayerProxy,Vector3>();
    /// <summary>
    /// A Stack to hold reference to the player proxies that have traveled out of the room
    /// </summary>
    private Stack<PlayerProxy> _outOfBoundsProxies = new Stack<PlayerProxy>();

    private void OnTriggerStay(Collider collider)
    {
        //check if the collision object is is a PlayerProxy
        PlayerProxy proxy = collider.GetComponent<PlayerProxy>();
        //if its not return 
        if (!proxy)
            return;
        //check if this proxy was already known of
        else if (!_knownPlayerProxies.ContainsKey(proxy))
        {
            //add it as it wasnt recognised before
            _knownPlayerProxies.Add(proxy, Vector3.zero);
            //Debug.Log($"{proxy.gameObject.name} was discovered");
        }
        //store its last known position
        _knownPlayerProxies[proxy] = collider.transform.position;
        //Debug.Log($"{proxy.gameObject.name};s last known position was {_knownPlayerProxies[proxy].ToString()}");

    }
    private void OnTriggerExit(Collider collider)
    {   
        //check if the exiting colider is a proxy
        PlayerProxy proxy = collider.GetComponent<PlayerProxy>();
        //if its not then return
        if(!proxy)
            return;

        //tell the proxy to return to its last position in the room
        _outOfBoundsProxies.Push(proxy);
        Debug.Log($"{proxy.gameObject.name} left the room");
    }

    private void LateUpdate()
    {
        //check if theres any player proxies that are out of bounds
        if (_outOfBoundsProxies.Count < 1)
            //there isnt so lets return
            return;
        //while the out of bounds stack isnt empty
        while(_outOfBoundsProxies.Count > 0)
        {
            //get the next out of bound player proxy
            PlayerProxy proxy = _outOfBoundsProxies.Pop();
            //get its last postion
            Vector3 lastKnownPosition = _knownPlayerProxies[proxy];
            proxy.HandleOutOfBounds(lastKnownPosition);
            Debug.Log($"{proxy.gameObject.name} returned to the room");
        }
    }

}

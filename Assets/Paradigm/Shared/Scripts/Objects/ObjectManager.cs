using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ObjectManager : Singleton<ObjectManager>, INetworkListener
{

    [SerializeField] private List<InteractableObject> _interactableObjectPrefabs;

    public List<InteractableObject> InteractableObjectPrefabs { get{ return _interactableObjectPrefabs; } }

    [SerializeField] private Transform _spawnPosition;

    private Dictionary<string,InteractableObject> _activeObjects = new Dictionary<string, InteractableObject>();

    private Dictionary<InteractableObject, ulong?> _ownedObjects = new Dictionary<InteractableObject, ulong?>(); 

    private void Awake()
    {
        SubscribeToConnectionManager();
    }

    public void SubscribeToConnectionManager()
    {
        ConnectionManager.Instance.SubscribeNetworkListener( this );
    }

    public void SetupNetworkMessages()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            if (NetworkManager.Singleton.IsServer)
                RegisterMessages();
        };
        //Register lambda event that executes when the a client connects to the server.
        NetworkManager.Singleton.OnClientConnectedCallback += (ulong clientID) =>
        {
            if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.LocalClientId == clientID)
                RegisterMessages();
        };

        //sneaking this line in here for convience
        //subscribe to the NetworkManager's OnClientDisconnect callback so we can ensure that object
        //ownership can be released
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnect;
    }

    private void RegisterMessages()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
            "SpawnObjectServerRequest", SpawnObjectServerRequest);
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
            "ChangeOwnershipOfObjectServerRequest", ChangeOwnershipOfObjectServerRequest);
    }


    public void RequestObjectSpawn(int objectIndex)
    {
        //if we're offline
        if (!ConnectionManager.IsOnline)
        {
            //spawn the object locally
            SpawnObject(objectIndex);
            return;
        }
        //else we're online so send message to the server to spawn the object client side

        if (objectIndex < 0 || objectIndex > _interactableObjectPrefabs.Count - 1)
        {
            return;
        }

        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(objectIndex), Allocator.Temp);

        using (writer)
        {
            //write the lookup data to the writer (pack the payload)
            writer.WriteValueSafe(objectIndex);
            //send the request to the server
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                //name of the message we want to send
                "SpawnObjectServerRequest",
                //address the message to the server 
                NetworkManager.ServerClientId,
                //insert the payload
                writer,
                //set the delivery method to reliable
                NetworkDelivery.Reliable
            );
        }

    }

    private void SpawnObjectServerRequest(ulong senderID, FastBufferReader messagePayload)
    {
        int objectIndex;

        //read and unpack the content of the payload
        messagePayload.ReadValueSafe(out objectIndex);
        
        //double check to make sure that the client and server have the same object prefab list
        if (objectIndex < 0 || objectIndex > _interactableObjectPrefabs.Count - 1)
        {
            return;
        }

        InteractableObject interactableObject = SpawnObject(objectIndex);

        NetworkObject networkObject = interactableObject.GetComponent<NetworkObject>();


        try
        {
            networkObject.Spawn();

            _activeObjects.Add(interactableObject.ID, interactableObject);
            _ownedObjects.Add(interactableObject,null);
        }
        catch(Exception e)
        {
            Debug.Log($"{e}");
        }
    }

    private void SpawnObjectClientBroadcast(ulong senderID, FastBufferReader messagePayload)
    {
        int objectIndex;

        messagePayload.ReadValueSafe(out objectIndex);

        //double check to make sure that the client and server have the same object prefab list
        if (objectIndex < 0 || objectIndex > _interactableObjectPrefabs.Count - 1)
        {
            return;
        }

        SpawnObject(objectIndex);
    }

    //create receive message

    private InteractableObject SpawnObject(int objectIndex)
    {
        if (objectIndex < 0 && objectIndex > _interactableObjectPrefabs.Count - 1)
        {
            Debug.Log($"OBJECT MANAGER - RequestObjectSpawn: Failed to spawn object prefab of index {objectIndex}");
            return null;
        }

        InteractableObject spawnedObject = Instantiate(
            _interactableObjectPrefabs[objectIndex],
            _spawnPosition.position,
            _spawnPosition.rotation
        );


        Debug.Log($"Instantiated object: {spawnedObject.name}");

        return spawnedObject;
    }


    public void RequestChangeOwnershipOfObject(InteractableObject targetObject, bool isRevokeRequest)
    {
        //get the id if this client
        ulong clientID = NetworkManager.Singleton.LocalClientId;

        //convert the Interactble ID to fixed string to serialise it
        FixedString64Bytes objectID = new FixedString64Bytes(targetObject.ID);

        ChangeOwnershipRequest request = new ChangeOwnershipRequest()
        {
            //mark if the request is to revoke ownership
            isRevokeRequest = isRevokeRequest,
            //Pass in the id of this client
            requestedOwnerID = clientID,
            //reference the ID of the interactable we want to own
            objectID = objectID,
        };

        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(request),Allocator.Temp);

        using (writer)
        {
            //write the lookup data to the writer (pack the payload)
            writer.WriteValueSafe(request);
            //send the request to the server
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                //name of the message we want to send
                "ChangeOwnershipOfObjectServerRequest",
                //address the message to the server 
                NetworkManager.ServerClientId,
                //insert the payload
                writer,
                //set the delivery method to reliable
                NetworkDelivery.Reliable
            );
        }
    }

    private void ChangeOwnershipOfObjectServerRequest(ulong senderID, FastBufferReader messagePayload)
    {
        ChangeOwnershipRequest request;

        messagePayload.ReadValueSafe(out request);


        //now we check if the object exists in the active objects dict
        if (!_activeObjects.TryGetValue(request.objectID.ToString(), out InteractableObject targetObject))
            return;

        //then we check who owns it currently
        if (!_ownedObjects.TryGetValue(targetObject, out ulong? ownerID))
            return;
        //check if this is a revoke request and if the requestee owns the object
        if (request.isRevokeRequest && _ownedObjects[targetObject] == request.requestedOwnerID)
        {
            Debug.Log($"Client({request.requestedOwnerID}) revokes ownership of object({targetObject.ID})");
            //it is a revoke request so remove ownership and give it back to the server
            _ownedObjects[targetObject] = null;
            targetObject.GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.ServerClientId);
            return;
        }
        else if(request.isRevokeRequest)
        {
            Debug.Log($"Client({request.requestedOwnerID}) revokes ownership of object({targetObject.ID})");
            return;
        }
        //so now we know that this is a transfer request so we should check if its available to own
        //if the owner ID isn't null then someone owns it so we have to wait
        if (ownerID != null )
        {
            Debug.Log($"Client({ownerID}) owns object: {targetObject.ID} so client({request.requestedOwnerID}) has to wait.");
            return;
        }
        //check if we are the host as if theres no owner, the object belongs already to the server (therefore the host)
        if (request.requestedOwnerID == NetworkManager.ServerClientId)
            return;

        Debug.Log($"Transfering ownership of Object({targetObject.ID}) from Client({ownerID}) to client({request.requestedOwnerID})");
        //transfer the ownership to the requested client
        _ownedObjects[targetObject] = request.requestedOwnerID;
        targetObject.GetComponent<NetworkObject>().ChangeOwnership(request.requestedOwnerID);
    }

    private void HandleDisconnect(ulong disconnectedClientID)
    {
        //check each object to see if the disconnected owns any of the interactables
        foreach(InteractableObject interactable in _activeObjects.Values)
        {
            //check if the interactable exists in the owned objects dict
            if (!_ownedObjects.TryGetValue(interactable, out ulong? ownerID))
                continue;
            //check if the owner of the interactable is the one who disconnected
            if (ownerID != disconnectedClientID)
                continue;
            //if it was the target client then set the ownership to null and give it back to the server
            _ownedObjects[interactable] = null;
            interactable.GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.ServerClientId);
        }
    }
    
}

public struct ChangeOwnershipRequest : INetworkSerializable
{
    public bool isRevokeRequest;

    public ulong requestedOwnerID;

    public FixedString64Bytes objectID;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref isRevokeRequest);
        serializer.SerializeValue(ref requestedOwnerID);
        serializer.SerializeValue(ref objectID);
    }
}

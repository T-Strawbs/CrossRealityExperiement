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

    private HashSet<InteractableObject> _activeObjects = new HashSet<InteractableObject>();

    public void RequestObjectSpawn(int objectIndex)
    {
        //if we're offline
        if(!ConnectionManager.IsOnline)
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
    }

    private void RegisterMessages()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
            "SpawnObjectServerRequest", SpawnObjectServerRequest);
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
            "SpawnObjectClientBroadcast", SpawnObjectClientBroadcast);
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
        }
        catch(Exception e)
        {
            Debug.Log($"{e}");
        }


        ////create a writer to contain our payload
        //var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(objectIndex), Allocator.Temp);

        //using (writer)
        //{
        //    //pack our payload
        //    writer.WriteValueSafe(objectIndex);
        //    //send the request to the clients to spawn the object at the given index
        //    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(
        //        //name of the message we want to send
        //        "SpawnObjectClientBroadcast",
        //        //insert the payload
        //        writer,
        //        //set the delivery method to reliable
        //        NetworkDelivery.Reliable
        //    );

        //}
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

    
}

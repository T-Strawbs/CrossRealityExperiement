using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;

public enum ConnectionStatus
{
    ONLINE,
    OFFLINE
}

public enum ConnectionType
{
    CLIENT,
    HOST,
    NONE
}

public class ConnectionManager : Singleton<ConnectionManager>
{
    public static ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.OFFLINE;
    public static bool IsOnline { get { return ConnectionStatus == ConnectionStatus.ONLINE; } }

    public static ConnectionType ConnectionType { get; private set; } = ConnectionType.NONE;
    public static bool IsHost { get { return ConnectionType == ConnectionType.HOST; } }
    public static bool IsClient { get { return ConnectionType == ConnectionType.CLIENT; } }


    private List<INetworkListener> _networkListeners = new List<INetworkListener>();


    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStart;
        NetworkManager.Singleton.OnServerStopped += OnServerStop;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        foreach (INetworkListener listener in _networkListeners)
            listener.SetupNetworkMessages();
    }

    public void SubscribeNetworkListener(INetworkListener networkListener)
    {
        if (_networkListeners.Contains(networkListener))
            return;
        _networkListeners.Add(networkListener);
    }

    public bool HandleConnection(string ip, string port, ConnectionType connectionType)
    {
        //validate the connection type and the ip and port strings
        if (connectionType == ConnectionType.NONE || !ValidateConnectionData(ip, port))
        {
            Debug.Log($"Connnection data invalid: IP: {ip} Port: {port} connectionType {connectionType}");
            return false;
        }
        //configure a new transport
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        //set the ip and port number of the connection
        transport.ConnectionData.Address = ip;
        if(!ushort.TryParse(port, out ushort portNumber))
        {
            Debug.Log($"Connnection data invalid: IP: {ip} Port: {port} connectionType {connectionType}");
            return false;
        }
        transport.ConnectionData.Port = portNumber;
        //handle Connection type
        if(connectionType == ConnectionType.HOST)
        {
            return StartServer();
        }
        else
        {
           return JoinServer();
        }
    }

    private void OnServerStart()
    {
        Debug.Log($"Server started");

        if (!NetworkManager.Singleton.IsServer)
            return;
    }
    private void OnServerStop(bool stopped)
    {
        Debug.Log($"Server stopped: {stopped}");
    }

    private void OnClientConnect(ulong id)
    {
        Debug.Log($"Client({id}) connected");

       
    }

    private void OnClientDisconnect(ulong id)
    {
        Debug.Log($"Client({id}) disconnected");
    }

    private bool StartServer()
    {
        string ip = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;
        string port = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port.ToString();
        Debug.Log($"Attempting to Host server on: {ip}:{port}");
        try
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log($"Hosting server: {ip}:{port}");

            ConnectionStatus = ConnectionStatus.ONLINE;
            ConnectionType = ConnectionType.HOST;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log($"Couldnt Host server on: {ip}:{port}");
            return false;
        }
    }

    private bool JoinServer()
    {
        string ip = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;
        string port = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port.ToString();
        Debug.Log($"Attempting to join server on: {ip}:{port}");
        try
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log($"joining server: {ip}:{port}");
            ConnectionStatus = ConnectionStatus.ONLINE;
            ConnectionType = ConnectionType.CLIENT;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log($"Couldnt join server on: {ip}:{port}");
            return false;
        }
    }

    public bool Disconnect()
    {
        Debug.Log($"disconnecting from server");
        try
        {
            NetworkManager.Singleton.Shutdown();
            ConnectionStatus = ConnectionStatus.OFFLINE;
            ConnectionType = ConnectionType.NONE;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    private bool ValidateConnectionData(string ip, string port)
    {
        bool validIP = false, validPort = false;

        string[] validateIP = ip.Split('.');
        if(validateIP.Length < 4)
        {
            validIP = false;
            Debug.Log($"Failure IP has less than 4 segments: {ip}");
        }
            
        foreach(string section in validateIP)
        {
            //check if the string is all digits
            if (int.TryParse(section, out int valueIP))
            {
                if (valueIP >= 0 && valueIP < 256)
                    validIP = true;
                else
                {
                    Debug.Log($"Failure section: ({section}) is invalid");
                    validIP = false;
                    break;
                }
            }
            else
            {
                Debug.Log($"Failure section: ({section}) is not an INT");
                validIP = false;
                break;
            }
        }
        if(int.TryParse(port, out int valuePort))
        {
            if (valuePort < 1)
            {
                Debug.Log($"Failure port: ({valuePort}) is < 1 ");
                validPort = false;
            }
            else 
                validPort = true;
        }
        else
        {
            Debug.Log($"Failure port ({valuePort}) is not an INT");
            validPort = false;
        }
            

        return validIP && validPort;

    }

    

}

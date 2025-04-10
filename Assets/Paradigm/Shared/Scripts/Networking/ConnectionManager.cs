using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;
using static System.Collections.Specialized.BitVector32;

public class ConnectionManager : NetworkSingleton<ConnectionManager>
{
    public void Awake()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStart;
        NetworkManager.Singleton.OnServerStopped += OnServerStop;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnServerStart()
    {
        Debug.Log($"Server started");
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
        if(connectionType == ConnectionType.SERVER)
        {
            return StartServer();
        }
        else
        {
           return JoinServer();
        }
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
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log($"Couldnt join server on: {ip}:{port}");
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

    public bool Disconnect()
    {
        Debug.Log($"disconnecting from server");
        try
        {
            NetworkManager.Singleton.Shutdown();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }
}

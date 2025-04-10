using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum ConnectionType
{
    SERVER,
    CLIENT,
    NONE
}

public class NetworkMenu : MenuUI
{
    [SerializeField] private Button _backButton;

    private ConnectionElement _currentElement;

    [SerializeField] private ConnectionTypeElement _connectionTypeElement;
    [SerializeField] private ConnectionDetailsElement _connectionDetailsElement;

    /// <summary>
    /// Enum for tracking the type of the current connection (SERVER or CLIENT)
    /// </summary>
    public ConnectionType CurrentConnectionType { get; private set; } = ConnectionType.NONE;

    public override string MenuName { get; protected set; } = "Network Menu";

    public override void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    public override void Initialise(MenuState handMenu)
    {
        _menuState = handMenu;
    }

    private void Awake()
    {
        if(_backButton)
            _backButton.onClick.AddListener(() => Back());

        _connectionTypeElement.Initialise(this);
        _connectionDetailsElement.Initialise(this);

        _connectionTypeElement.gameObject.SetActive(true);
        _connectionDetailsElement.gameObject.SetActive(false);

        _currentElement = _connectionTypeElement;

    }

    private void Back()
    {
        _menuState.ChangeMenu(MenuStateEnum.MAIN);
    }

    public void UpdateConnectionDetails(ConnectionType connectionType)
    {
        CurrentConnectionType = connectionType;
        _connectionDetailsElement.UpdateConnectionType(connectionType);
        ChangeConnectionElement();
    }

    public void ChangeConnectionElement()
    {
        //deactivate the current element
        _currentElement.Deactivate();
        //check the type of the current connection element and change it
        //to the other one
        if(_currentElement == _connectionTypeElement)
            _currentElement = _connectionDetailsElement;
        else
            _currentElement = _connectionTypeElement;
        //activate the current element
        _currentElement.Activate();
    }

    public bool RequestConnection(string ip, string port)
    {
        //attempt to establish a connection using the connection data
        bool isConnected = ConnectionManager.Instance.HandleConnection(ip, port, CurrentConnectionType);
        //toggle the connection state UI of the ConnectionDetailsElement
        _connectionDetailsElement.ToggleConnectionState(isConnected);

        return isConnected;
    }

    public bool RequestDisconnect()
    {
        //attempt to disconnect
        bool isConnected = ConnectionManager.Instance.Disconnect();
        //toggle the connection state UI of the ConnectionDetailsElement
        _connectionDetailsElement.ToggleConnectionState(isConnected);

        return isConnected;
    }
}
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

/// <summary>
/// Connection Element Concretion for capturing the details of the future connection
/// </summary>
public class ConnectionDetailsElement : ConnectionElement
{
    /// <summary>
    /// Input field for setting the IP of  the future connection
    /// </summary>
    [SerializeField] private TMP_InputField _ipInputField;
    /// <summary>
    /// Input field for setting the Port of  the future connection
    /// </summary>
    [SerializeField] private TMP_InputField _portInputField;
    /// <summary>
    /// The Element that parents both the IpInputField and PortInputFields
    /// </summary>
    [SerializeField] private RectTransform _detailsRow;
    /// <summary>
    /// The button element for returning back to the connection type element
    /// </summary>
    [SerializeField] private Button _backButton;
    /// <summary>
    /// The button element used to invoke the connection process
    /// </summary>
    [SerializeField] private Button _connectButton;
    /// <summary>
    /// The text element of the ConnectButton
    /// </summary>
    [SerializeField] private TMP_Text _connectButtonText;
    /// <summary>
    /// The button element for invoking the disconnection process if we are connected
    /// </summary>
    [SerializeField] private Button _disconnectButton;

    public override void Initialise(NetworkMenu networkMenu)
    {
        _networkMenu = networkMenu;
    }
    public override void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    private void Awake()
    {
        //set the onClick event listeners of each button
        _backButton.onClick.AddListener(() => _networkMenu.ChangeConnectionElement());
        _connectButton.onClick.AddListener(() =>
            _networkMenu.RequestConnection(_ipInputField.text,_portInputField.text));
        _disconnectButton.onClick.AddListener(() => {
            _networkMenu.RequestDisconnect();
            ToggleConnectionState(false);
        });
    }

    public void UpdateConnectionType(ConnectionType connectionType)
    {
        if(connectionType == ConnectionType.SERVER)
        {
            _connectButtonText.text = "Host";
        }
        else
        {
            _connectButtonText.text = "Join";
        }
    }    

    /// <summary>
    /// Method for toggling the state of the connection UI
    /// </summary>
    public void ToggleConnectionState(bool isConnected)
    {
        //Sets the detail data inactive if we have a connection or active if we dont
        _detailsRow.gameObject.SetActive(!isConnected);
        _connectButton.gameObject.SetActive(!isConnected);
        //Sets the disconnect button inactive if we dont have a connection or active
        //if we do 
        _disconnectButton.gameObject.SetActive(isConnected);
    }    



}
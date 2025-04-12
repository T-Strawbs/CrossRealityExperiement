using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionTypeElement : ConnectionElement
{
    [SerializeField] private Button _clientBtn;
    [SerializeField] private Button _ServerBtn;

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
        _clientBtn.onClick.AddListener(() => {
            _networkMenu.UpdateConnectionDetails(ConnectionType.CLIENT);
        });
        _ServerBtn.onClick.AddListener(() => {
            _networkMenu.UpdateConnectionDetails(ConnectionType.HOST);
        });
    }


}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MenuUI
{
    [SerializeField] private Button _backButton;
    public override string MenuName { get; protected set; } = "Debug Menu";

    [SerializeField] private TMP_Text _debugContent;

    public override void Activate()
    {
       this.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    public override void Initialise(MenuState menuState)
    {
        _menuState = menuState;
        if (_backButton)
            _backButton.onClick.AddListener(() => { _menuState.ChangeMenu(MenuStateEnum.MAIN); });
    }

    private void Awake()
    {
        _debugContent.text = "";
        Application.logMessageReceived += PrintDebugMessage;
        
    }


    private void Start()
    {
        Debug.Log("Hello World from the Debug Menu");
    }
    private void PrintDebugMessage(string logString, string stackTrace, LogType type)
    {
        _debugContent.text += $"• [{type}]: {logString}\n-- stack: {stackTrace}\n";
    }
}

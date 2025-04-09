using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ToggleState
{
    ON,
    OFF,
    INACTIVE
}

public class ToggleButton : MonoBehaviour
{
    [SerializeField] protected Button _toggle;
    [SerializeField] protected Image _background;
    public ToggleState ToggleState { get; protected set; } = ToggleState.OFF;

    public event Action<ToggleButton,ToggleState> onValueChanged;

    public void TurnOn()
    {
        ToggleState = ToggleState.ON;

        UpdateVisuals();
    }

    public void TurnOff()
    {
        ToggleState = ToggleState.OFF;

        UpdateVisuals();
    }

    public void Activate()
    {
        ToggleState = ToggleState.OFF;

        UpdateVisuals();
    }

    public void Deactivate()
    {
        ToggleState = ToggleState.INACTIVE;

        UpdateVisuals();
    }
    private void Awake()
    {
        _toggle.onClick.AddListener(() => Toggle());
    }

    private void Toggle()
    {
        if (ToggleState == ToggleState.INACTIVE)
            return;

        ToggleState = ToggleState == ToggleState.ON ? ToggleState.OFF : ToggleState.ON;

        UpdateVisuals();
        onValueChanged?.Invoke(this, ToggleState);
    }
    
    private void UpdateVisuals()
    {
        switch(ToggleState)
        {
            case ToggleState.ON:
                _background.color = Color.cyan;
                break;
            case ToggleState.OFF:
                _background.color = Color.white;
                break;
            case ToggleState.INACTIVE:
                _background.color = Color.gray;
                break;
        }
    }

}
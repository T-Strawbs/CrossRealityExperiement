using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectOption : MonoBehaviour
{
    [SerializeField] private Button _spawnButton;
    [SerializeField] private TMP_Text _objectNameText;
    private int _objectIndex;

    public void Initialise(string objectName, int objectIndex)
    {
        //set the name text to the object name
        _objectNameText.text = objectName;
        //set the object index
        _objectIndex = objectIndex;
    }

    private void Awake()
    {
        _spawnButton.onClick.AddListener(() => InvokeSpawn());
    }

    private void InvokeSpawn()
    {
        //request the server to spawn the interactable object
        ObjectManager.Instance.RequestObjectSpawn(_objectIndex);
    }
}

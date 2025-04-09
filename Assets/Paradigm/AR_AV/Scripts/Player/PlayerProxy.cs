using UnityEngine;

/// <summary>
/// The class for the objects acting as a positional proxy for the player transform.
/// </summary>
public class PlayerProxy : MonoBehaviour
{

    [SerializeField] private Transform _cameraOffsetTransform;

    public void HandleOutOfBounds(Vector3 lastPosition)
    {
        this.transform.position = lastPosition;
        _cameraOffsetTransform.position = lastPosition;
    }

}

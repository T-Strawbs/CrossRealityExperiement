using System;
using System.Collections.Generic;
using UnityEngine;
public interface RoomObject
{
    public string Name { get; }
    public void HandleOutOfBounds(Vector3 lastPosition);
}
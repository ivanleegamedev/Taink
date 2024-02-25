using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    Rigidbody rb { get; set; }
    void MoveEnemy(Vector2 velocity);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    public abstract void OnStateEnter(GameManager game);
    public abstract void OnStateExit(GameManager game);
    public abstract void OnStateUpdate(GameManager game);
    public abstract void OnStateFixedUpdate(GameManager game);
    public abstract void OnStateCollisionEnter(GameManager game, Collision collision);
    public abstract void OnStateTriggerEnter(GameManager game, Collider collision);
}

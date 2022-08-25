using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningGameState : GameState
{
    public override void OnStateCollisionEnter(GameManager game, Collision collision)
    {
        
    }

    public override void OnStateEnter(GameManager game)
    {
        
    }

    public override void OnStateExit(GameManager game)
    {
        
    }

    public override void OnStateFixedUpdate(GameManager game)
    {
        game.SpawnEnemies();
    }

    public override void OnStateTriggerEnter(GameManager game, Collider collision)
    {
        
    }

    public override void OnStateUpdate(GameManager game)
    {
        game.ActivateEnemyAndDrawPath();
        game.DisplayTowerRange();
        if (!game._gameStarted) game.TransitionToState(game._planningState);
        if (game._spawnFinished) game.TransitionToState(game._runningState);
    }
}

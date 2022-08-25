using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningGameState : GameState
{
    public override void OnStateCollisionEnter(GameManager game, Collision collision)
    {
        
    }

    public override void OnStateEnter(GameManager game)
    {
        
    }

    public override void OnStateExit(GameManager game)
    {
        //if (!game.CheckVictoryCondition()) game.ResetGameElements();
    }

    public override void OnStateFixedUpdate(GameManager game)
    {
        game.CheckGoalReached();
    }

    public override void OnStateTriggerEnter(GameManager game, Collider collision)
    {
        
    }

    public override void OnStateUpdate(GameManager game)
    {
        game.ActivateEnemyAndDrawPath();
        game.DisplayTowerRange();
        if (!game._gameStarted) game.TransitionToState(game._planningState);
        if (game.CheckVictoryCondition()) game.TransitionToState(game._victoryState);
        else if (game.CheckEndGameCondition()) game.TransitionToState(game._planningState);
    }
}

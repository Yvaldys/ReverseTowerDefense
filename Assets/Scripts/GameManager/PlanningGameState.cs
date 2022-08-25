using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningGameState : GameState
{
    public override void OnStateCollisionEnter(GameManager game, Collision collision)
    {
        
    }

    public override void OnStateEnter(GameManager game)
    {
        //game.DrawPlanningPath();
        game.ResetGameElements();
    }

    public override void OnStateExit(GameManager game)
    {
        
    }

    public override void OnStateFixedUpdate(GameManager game)
    {
        
    }

    public override void OnStateTriggerEnter(GameManager game, Collider collision)
    {
        
    }

    public override void OnStateUpdate(GameManager game)
    {
        game.DisplayTowerRange();
        if (game._gameStarted) game.TransitionToState(game._spawningState);
    }
}

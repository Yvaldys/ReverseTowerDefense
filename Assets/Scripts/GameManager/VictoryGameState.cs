using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryGameState : GameState
{
    public override void OnStateCollisionEnter(GameManager game, Collision collision)
    {
        
    }

    public override void OnStateEnter(GameManager game)
    {
        game.CleanPathAndTowerRange();
        game._onVictory.Invoke();
        game.UnlockNextLevel();
        game.SaveScore();
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
        
    }
}

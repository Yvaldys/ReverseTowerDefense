using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ScoresList
{
    #region Singleton
    private static ScoresList instance;
    public static ScoresList Instance
    {
        get
        {
            if (instance == null)
                instance = SaveSystem.LoadScoresList();
            if (instance == null || instance.starsUnlocked.Length != 50)
                instance = new ScoresList();
            return instance;
        }
    }
    #endregion

    private ScoresList() {
        starsUnlocked = new int[50];
        // lock all levels (not level 1)
        for (int i = 2; i < starsUnlocked.Length; i++) {
            starsUnlocked[i] = -1;
        }
    }

    public int[] starsUnlocked;

}

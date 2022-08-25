using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetData : MonoBehaviour
{
    #region Exposed
    
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    #endregion

    #region Main methods
    public void DestroySaveAndReload() {
        SaveSystem.DestroySaveData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

    #region Private & Protected
    
    #endregion
}

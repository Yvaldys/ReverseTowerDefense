using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPanelScript : MonoBehaviour
{
    #region Exposed
    [SerializeField] private GameObject _victorypanel;
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        
    }

    void Start()
    {
        GameManager.Instance._onVictory += DisplayPanel;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    #endregion

    #region Main methods
    private void DisplayPanel() {
        _victorypanel.SetActive(true);
    }
    #endregion

    #region Private & Protected
    
    #endregion
}

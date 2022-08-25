using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPanelScript : MonoBehaviour
{
    #region Exposed
    [SerializeField] private GameObject _levelButtonPrefab;
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        //SaveSystem.DestroySaveData();
        _transform = this.transform;
        int[] stars = ScoresList.Instance.starsUnlocked;

        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++) {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            //if (sceneName == SceneManager.GetActiveScene().name) continue;

            GameObject levelButtonObject = Instantiate(_levelButtonPrefab, _transform);
            // name
            levelButtonObject.GetComponentInChildren<Text>().text = sceneName;

            // button action
            Button levelButton = levelButtonObject.GetComponent<Button>();
            levelButton.onClick.AddListener(() => {SceneManager.LoadScene(sceneName);});

            // stars
            /*always enable level 1*/
            if (stars[i] == -1) levelButton.interactable = false;
            else {
                for (int j = 0; j < stars[i]; j++) {
                    GameObject fill = levelButtonObject.transform.Find("Stars Panel").GetChild(j).GetChild(0).gameObject;
                    fill.SetActive(true);
                }
            }
        }
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
    
    #endregion

    #region Private & Protected
    private Transform _transform;
    #endregion
}

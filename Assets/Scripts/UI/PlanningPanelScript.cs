using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanningPanelScript : MonoBehaviour
{
    #region Exposed
    [SerializeField] private GameObject _stickerPrefab;
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        _transform = transform;
    }

    void Start()
    {
        GameManager.Instance._onResetElements += PressStop;
        foreach (EnemyInfo enemyInfo in GameManager.Instance._enemyinfos) {
            GameObject sticker = Instantiate(_stickerPrefab, _transform);
            sticker.gameObject.name = enemyInfo._name;

            Image stickerImage = sticker.GetComponent<Image>();
            stickerImage.color = DamageClassColor.GetColor(enemyInfo._resistType);

            if (enemyInfo._specialImage != null) {
                Image SpecialImage = sticker.transform.GetChild(0).GetComponent<Image>();
                SpecialImage.enabled = true;
                SpecialImage.sprite = enemyInfo._specialImage;
            }
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    #endregion

    #region Main methods
    public void StartGame() {
        string[] enemyNames = new string[_transform.childCount];
        int i = 0;
        foreach (Transform child in _transform) {
            enemyNames[i] = child.name;
            i++;
        }
        GameManager.Instance.SetEnemySpawnInfos(enemyNames);
        GameManager.Instance._gameStarted = true;
    }

    public void StopGame() {
        GameManager.Instance._gameStarted = false;
    }

    public void PressStop() {
        Transform stopButton = _transform.parent.Find("StartStop/Stop Game");
        if (stopButton != null && stopButton.gameObject.activeSelf) stopButton.GetComponent<Button>().onClick.Invoke();
    }
    #endregion

    #region Private & Protected
    private Transform _transform;
    #endregion
}

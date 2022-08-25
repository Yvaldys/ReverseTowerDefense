using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarScript : MonoBehaviour
{
    #region Exposed
    public GameObject _progressBarFill;
    public Transform _starPanel;
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        
    }

    void Start()
    {
        GameManager.Instance._onEnemyCountChange += UpdateHUD;
        _fillImage = _progressBarFill.GetComponent<Image>();

        // position for indicators
        float fillWidth = _progressBarFill.GetComponent<RectTransform>().rect.width;
        float[] starPositionsX = new float[3];
        starPositionsX[0] = (GameManager.Instance._firstStar / GameManager.Instance._totalEnemy) * fillWidth;
        starPositionsX[1] = (GameManager.Instance._secondStar / GameManager.Instance._totalEnemy) * fillWidth;
        starPositionsX[2] = (GameManager.Instance._thirdStar / GameManager.Instance._totalEnemy) * fillWidth;

        for (int i = 0; i < 3; i++) {
            Transform indicatorTransform = _progressBarFill.transform.GetChild(i);
            Vector3 newPosition = new Vector3(indicatorTransform.localPosition.x+starPositionsX[i], indicatorTransform.localPosition.y, indicatorTransform.localPosition.z);
            indicatorTransform.localPosition = newPosition;
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
    public void UpdateHUD(float enemyCount) {
        _fillImage.fillAmount = enemyCount / GameManager.Instance._totalEnemy;
        for (int i = 0; i < GameManager.Instance._currentScore; i++) {
            _starPanel.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
        /*if (enemyCount == GameManager.Instance._firstStar) {
            _starPanel.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
        if (enemyCount == GameManager.Instance._secondStar) {
            _starPanel.GetChild(1).GetChild(0).gameObject.SetActive(true);
        }
        if (enemyCount == GameManager.Instance._thirdStar) {
            _starPanel.GetChild(2).GetChild(0).gameObject.SetActive(true);
        }*/
    }
    #endregion

    #region Private & Protected
    private Image _fillImage;
    #endregion
}

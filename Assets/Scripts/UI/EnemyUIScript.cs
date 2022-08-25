using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUIScript : MonoBehaviour
{
    #region Exposed
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private Image _healthBarFill;
    [SerializeField] private float _damageAnimationDuration = 0.1f;
    [SerializeField] private float _damageAnimationHeight = 0.3f;
    [SerializeField] private GameObject _healthModificatorPrefab;
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        
    }

    void Start()
    {
        originalRotation = transform.rotation;
        
        
        EnemyController enemyController = GetComponentInParent<EnemyController>();
        enemyController._OnHealthChange += UpdateHealthUI;
    }

    void Update()
    {
         transform.rotation = Camera.main.transform.rotation * originalRotation;
    }

    void FixedUpdate()
    {
        
    }

    #endregion

    #region Main methods
    private void UpdateHealthUI(float health, float maxHealth, float damage) {
        _healthBarFill.fillAmount = health/maxHealth;
        if (_healthBarFill.fillAmount >= 1) {
            _healthBar.SetActive(false);
        } else {
            _healthBar.SetActive(true);
        }

        StartCoroutine(DisplayDamage(damage));
    }

    IEnumerator DisplayDamage(float damage) {
        GameObject healtModificator = Instantiate(_healthModificatorPrefab, transform);
        TextMeshProUGUI text = healtModificator.GetComponent<TextMeshProUGUI>();

        Vector3 originalPos = healtModificator.transform.position;
        Vector3 offset = new Vector3(0, _damageAnimationHeight, 0);

        damage *= -1;
        if (damage < 0) text.color = Color.red;
        else text.color = Color.green;
        text.text = damage+"";
        
        float elapsedTime = 0;
        while (elapsedTime < _damageAnimationDuration) {
            healtModificator.transform.position  = Vector3.Lerp(originalPos, originalPos+offset, elapsedTime / _damageAnimationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(healtModificator);
    }
    #endregion

    #region Private & Protected
    private Quaternion originalRotation;
    #endregion
}

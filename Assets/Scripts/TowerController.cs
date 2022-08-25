using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    #region Exposed
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _fireDelay = 0.5f;
    public float _range = 2f;

    [Header("bullet parameters")]
    [SerializeField] private float _speed = 6;
    [SerializeField] private float _damage = 10;
    [Range(0,100)][SerializeField] private float _slowPercentage = 0;
    [SerializeField] private float _slowDuration = 2;
    [SerializeField] private bool _isAreaDamage = false;
    [SerializeField] private float _impactRadius = 0;
    [SerializeField] private DamageType _damageType = DamageType.NONE;
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        
    }

    void Start()
    {
        GameManager.Instance._onResetElements += CleanTargetlist;
        this.GetComponent<SphereCollider>().radius = _range;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (_enemyTargeted == null && _nextTargets.Count != 0) {
            _enemyTargeted = _nextTargets[0];
            _nextTargets.Remove(_enemyTargeted);
        }

        if (_enemyTargeted != null && Time.time - _lastShotTime >= _fireDelay) {
            _weaponTransform.LookAt(_enemyTargeted.transform.position);
            Fire();
        }
    }

    void OnTriggerEnter(Collider hit) {
        if (hit.CompareTag("Enemy")) {
                _nextTargets.Add(hit.gameObject);
        } 
    }

    void OnTriggerExit(Collider hit) {
        if (hit.gameObject == _enemyTargeted) {
                _enemyTargeted = null;

        } else if (_nextTargets.Contains(hit.gameObject)) {
            _nextTargets.Remove(hit.gameObject);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    #endregion

    #region Main methods
    private void Fire() {
        _lastShotTime = Time.time;
        GameObject bullet = Instantiate(_bulletPrefab, _weaponTransform);
        bullet.GetComponent<TowerBullet>().SetBulletParameters(_speed, _damage, _slowPercentage, _slowDuration, _isAreaDamage, _impactRadius, _damageType, _enemyTargeted);
    }

    public void CleanTargetlist() {
        _nextTargets.Clear();
    }
    #endregion

    #region Private & Protected
    private GameObject _enemyTargeted;
    private List<GameObject> _nextTargets = new List<GameObject>();
    private float _lastShotTime;
    #endregion
}

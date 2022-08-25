using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    #region Exposed
    
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        _transform = this.transform;
        _rb = this.GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        _rb.velocity = _transform.forward * _speed;
    }

    void OnDestroy() {
        
    }

    void OnTriggerEnter(Collider hit) {
        if (hit.gameObject == _enemyTargeted) {
            if (_isAreaDamage) {
                Collider[] colliders = Physics.OverlapSphere(_transform.position, _impactRadius);
                
                foreach (Collider collider in colliders) {
                    if (collider.CompareTag("Enemy")) {
                        DamageEnemy(collider.GetComponent<EnemyController>());

                    }
                }
            } else {
                DamageEnemy(hit.GetComponent<EnemyController>());
            }
            Destroy(this.gameObject);
        }
    }

    void OnDrawGizmos() {
        if (_isAreaDamage) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_transform.position, _impactRadius);
        }
    }

    #endregion

    #region Main methods
    private void DamageEnemy(EnemyController controller) {
        controller.TakeDamage(_damage, _damageType);
        controller.Slow(_slowPercentage, _slowDuration);
    }

    public void SetBulletParameters(float speed, float damage, float slowPercentage, float slowDuration, bool isAreaDamage, float impactRadius, DamageType damageType, GameObject enemyTargeted) {
        _speed = speed;
        _damage = damage;
        _slowPercentage = slowPercentage;
        _slowDuration = slowDuration;
        _isAreaDamage = isAreaDamage;
        _impactRadius = impactRadius;
        _damageType = damageType;
        _enemyTargeted = enemyTargeted;
    }
    #endregion

    #region Private & Protected
    private Transform _transform;
    private Rigidbody _rb;
    private float _speed = 6;
    private float _damage = 10;
    private float _slowPercentage = 0;
    private float _slowDuration = 2;
    private bool _isAreaDamage = false;
    private float _impactRadius;
    private DamageType _damageType = DamageType.NONE;
    private GameObject _enemyTargeted;
    #endregion
}

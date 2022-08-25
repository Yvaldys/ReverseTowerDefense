using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EnemyType {GOBELIN, GARGOYLE}
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
public class EnemyController : MonoBehaviour
{
    #region Exposed
    [SerializeField] private float _health = 50;
    [SerializeField] private DamageType _damageTypeResist = DamageType.NONE;
    [SerializeField] private Image _specialImage;
    [SerializeField] private GameObject _wallPrefab;
    #endregion

    #region Events
    public delegate void OnHealthChange(float health, float maxHealth, float damage);
    public OnHealthChange _OnHealthChange;
    public delegate void EnemyAction();
    public EnemyAction _enemyAction;
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _originalSpeed = _agent.speed;
        _maxHealth = _health;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (_isSlowed && Time.time - _slowStartTime > _slowDuration) {
            _agent.speed = _originalSpeed;
            _isSlowed = false;
        }
    }

    #endregion

    #region Main methods

    public void TakeDamage(float damage, DamageType damageType) {
        // type modificator
        if (damageType == DamageType.NONE || _damageTypeResist == DamageType.NONE) {}
        else if (damageType == _damageTypeResist) {damage*= 0.5f;}
        else  damage*= 1.5f;
        damage = Mathf.Ceil(damage);
        
        _health -= damage;
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        if (_health <= 0) Destroy(this.gameObject);
        _OnHealthChange.Invoke(_health, _maxHealth, damage);
    }

    public void TakeDamageTestDebug() {
        TakeDamage(20, DamageType.NONE);
    }

    /*public void Heal(int amount) {
        _health += amount;
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        _OnHealthChange.Invoke(_health, _maxHealth, amount);
    }*/

    public void Slow(float slowPercentage, float slowDuration) {
        _isSlowed = true;
        _agent.speed = _originalSpeed * (1-slowPercentage/100);
        _slowStartTime = Time.time;
        _slowDuration = slowDuration;
    }

    public void GargoyleBehaviour() {
        //NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
        /*if (_agent.enabled && _destination == null) {
            _destination = _agent.destination;
        }*/
        if (_wallPrefab) StartCoroutine(GargoyleCoroutine());
        /*_agent.enabled = !_agent.enabled;
        obstacle.enabled = !obstacle.enabled;
        _agent.SetDestination(_destination);*/
    }

    IEnumerator GargoyleCoroutine() {
        /*NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
        if (_agent.enabled) {
            _agent.enabled = false;
            yield return new WaitForSeconds(0.1f);
            obstacle.enabled = true;
        } else {
            GameObject invisibleWall = Instantiate(this.gameObject, transform.position, Quaternion.identity);
            invisibleWall.SetActive(false);
            invisibleWall.transform.Find("Graphics").GetComponent<Renderer>().enabled = false;
            invisibleWall.transform.Find("Canvas").gameObject.SetActive(false);

            obstacle.enabled = false;
            yield return new WaitForSeconds(0.1f);
            _agent.enabled = true;
            _agent.SetDestination(_destination);
            invisibleWall.SetActive(true);
        }*/
        Vector3 wallPosition = transform.position;
        _specialImage.enabled = false;
        yield return new WaitForSeconds(0.1f);
        GameObject wall = Instantiate(_wallPrefab, wallPosition, transform.rotation, transform.parent);
        Destroy(wall, 10);

        _wallPrefab = null;
    }

    public void SetParameters(EnemyInfo enemyInfo) {
        this.gameObject.name = enemyInfo._name;
        if (enemyInfo._health != 0) {
            _maxHealth = _health = enemyInfo._health;
        }
        if (enemyInfo._speed != 0) {
            _agent.speed = _originalSpeed = enemyInfo._speed;
        }
        if (enemyInfo._resistType != DamageType.NONE) {
            _damageTypeResist =  enemyInfo._resistType;
            this.transform.Find("Graphics").GetComponent<Renderer>().material.color = DamageClassColor.GetColor(_damageTypeResist);
        }
        if (enemyInfo._specialImage != null) {
            //Image specialimage = this.transform.Find("Canvas/Panel/Special Image").GetComponent<Image>();
            _specialImage.sprite = enemyInfo._specialImage;
            _specialImage.enabled = true;
        }

        // switch for enemy types
        switch(enemyInfo._enemyType) {
            case EnemyType.GARGOYLE :
                /*EventTrigger enemyTrigger = this.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener( (eventData) => { GargoyleBehaviour(); } );
                enemyTrigger.triggers.Add(entry);*/
                _enemyAction += GargoyleBehaviour;
                break;
            default :
                break;
        }
    }

    public void SetDestination(Vector3 position) {
        _destination = position;
        _agent.SetDestination(position);
    }

    #endregion

    #region Private & Protected
    private NavMeshAgent _agent;
    private Vector3 _destination;
    //private LineRenderer _linePath;
    private bool _isSlowed, _isSelected;
    private float _originalSpeed, _slowStartTime, _slowDuration, _maxHealth;
    #endregion
}

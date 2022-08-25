using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyInfo : ScriptableObject
{
    public string _name;
    public GameObject _enemyPrefab;
    public float _health;
    public float _speed;
    public EnemyType _enemyType;
    public DamageType _resistType;
    public Sprite _specialImage;

}

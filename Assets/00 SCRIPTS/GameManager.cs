using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // References to important game objects
    [SerializeField] PlayerController _player;
    public PlayerController Player => _player;

    [SerializeField] EnemyMelee _enemyMelee;
    public EnemyMelee EnemyMelee => _enemyMelee;

    [SerializeField] RangedEnemy _rangedEnemy;
    public RangedEnemy RangedEnemy => _rangedEnemy;
}

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


    [SerializeField] private int _upgradePoints = 0;
    public int UpgradePoints => _upgradePoints;
    [SerializeField] private int _hpUpgradeCost = 10;
    public int HpUpgradeCost => _hpUpgradeCost;
    [SerializeField] private int _damageUpgradeCost = 5;
    public int DamageUpgradeCost => _damageUpgradeCost;
    [SerializeField] private int _healUpgradeCost = 5;
    public int HealUpgradeCost => _healUpgradeCost;

    private void Start()
    {
        _upgradePoints = PlayerPrefs.GetInt(CONSTANT.POINTS, 0);
        UIManager.Instance.UpdatePointText(_upgradePoints);
    }

    public void AddUpgradePoint(int amount)
    {
        _upgradePoints += amount;
        PlayerPrefs.SetInt(CONSTANT.POINTS, _upgradePoints);
        UIManager.Instance.UpdatePointText(_upgradePoints);
        //Debug.Log("Upgrade Points: " + _upgradePoints);
    }
}

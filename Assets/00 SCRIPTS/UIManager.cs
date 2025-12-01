using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    // Soul UI
    [SerializeField] private Slider _soulSlider;
    [SerializeField] private TMP_Text _soulText;

    // Health UI
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TMP_Text _healthText;

    // Stats Panel
    [SerializeField] public GameObject _statsPanel;
    [SerializeField] private TMP_Text _pointText;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _attackText;
    [SerializeField] private TMP_Text _healText;

    // Boss Health UI
    [SerializeField] private Slider _bossHealthSlider;
    public Slider BossHealthSlider => _bossHealthSlider;


    private void Start()
    {
        _statsPanel.SetActive(false); // an panel thong so luc dau
    }

    private void Update()
    {
        // Toggle Stats Panel
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowStatsPanel();
        }
    }

    public void ShowStatsPanel()
    {
        _statsPanel.SetActive(!_statsPanel.activeSelf);
    }

    // Update Soul Slider
    public void UpdateSoulSlider(int current, int max)
    {
        _soulSlider.maxValue = max; // cap nhat gia tri toi da
        _soulSlider.value = current; // cap nhat gia tri hien tai
        _soulText.text = _soulSlider.value + "/" + _soulSlider.maxValue; // cap nhat text
    }

    // Update Health Slider
    public void UpdateHealthSlider(int current, int max)
    {
        _healthSlider.maxValue = max; // cap nhat gia tri toi da
        _healthSlider.value = current; // cap nhat gia tri hien tai
        _healthText.text = _healthSlider.value + "/" + _healthSlider.maxValue; // cap nhat text
    }

    public void UpdateBossHealthSlider(int current, int max)
    {
        _bossHealthSlider.maxValue = max; // cap nhat gia tri toi da
        _bossHealthSlider.value = current; // cap nhat gia tri hien tai
    }

    // Update Point Text
    public void UpdatePointText(int point)
    {
        _pointText.text = "Points: " + point;
    }

    public void ShowHPText(int health)
    {
        _hpText.text = "HP: " + health;
    } 
    public void ShowAttackText(int attack)
    {
        _attackText.text = "Atk: " + attack;
    }
    public void ShowHealText(int heal)
    {
        _healText.text = "Heal: " + heal;
    }

}

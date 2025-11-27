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

}

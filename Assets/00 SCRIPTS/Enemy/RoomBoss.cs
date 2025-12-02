using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBoss : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.BossHealthSlider.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(CONSTANT.PLAYER_TAG))
        {
            UIManager.Instance.BossHealthSlider.gameObject.SetActive(true);
        }
    }
}

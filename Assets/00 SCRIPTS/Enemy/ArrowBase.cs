using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBase : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _lifetime;
    [SerializeField] int _damage;

    //[SerializeField] GameObject _hitFX;

    Rigidbody2D _rigi;

    // Start is called before the first frame update
    void Start()
    {
        _rigi = this.GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        StartCoroutine(AutoDestrust());
    }

    // Update is called once per frame
    void Update()
    {
        _rigi.velocity = _speed * this.transform.right;
    }

    IEnumerator AutoDestrust()
    {
        yield return new WaitForSeconds(_lifetime);
        this.gameObject.SetActive(false);
        //Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(CONSTANT.PLAYER_TAG))
        {
            // xu ly sat thuong o day
            GameManager.Instance.Player.TakeDamage(_damage, this.transform);
        }
        this.gameObject.SetActive(false);
    }
}

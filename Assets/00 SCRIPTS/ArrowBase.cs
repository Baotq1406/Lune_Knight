using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBase : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _lifetime;

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
}

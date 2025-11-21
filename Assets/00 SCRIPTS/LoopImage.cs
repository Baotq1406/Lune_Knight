using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopImage : MonoBehaviour
{
    Texture _texture;
    [SerializeField] int _pixelsPerUnit;
    float _imageWidth = 0;

    Transform _playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        _texture = this.GetComponent<SpriteRenderer>().sprite.texture;
        _playerTransform = GameManager.Instance.Player.transform;
        _imageWidth = (float)_texture.width / _pixelsPerUnit;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(_playerTransform.position.x - this.transform.position.x) >= _imageWidth)
        {
            Vector3 pos = this.transform.position;
            pos.x = _playerTransform.position.x;

            this.transform.position = pos;

        }
    }
}

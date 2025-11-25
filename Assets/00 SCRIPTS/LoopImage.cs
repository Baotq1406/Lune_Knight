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
        // Kiem tra xem hinh nen da cach xa player bao nhieu
        // Neu khoang cach giua background va player lon hon hoac bang do rong cua hinh
        // => tuc la background da "ra khoi man hinh" va can duoc dua ve lai phia truoc de tao hieu ung loop
        if (Mathf.Abs(_playerTransform.position.x - this.transform.position.x) >= _imageWidth)
        {
            // Lay vi tri hien tai cua background
            Vector3 pos = this.transform.position;

            // Dat lai vi tri x cua background bang vi tri x cua player
            // Dieu nay giup hinh nen lap lai mot cach lien tuc ma khong bi khoang trong
            pos.x = _playerTransform.position.x;

            // Gan lai vi tri moi cho background
            this.transform.position = pos;
        }
    }
}

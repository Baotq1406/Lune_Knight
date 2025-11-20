using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform _player;
    [SerializeField] Vector2 _camOffset;

    void Start()
    {
        _player = GameManager.Instance.Player.transform;
    }

    void Update()
    {
        //Vector3 newPos = new Vector3(_player.position.x, _player.position.y, this.transform.position.z);
        //this.transform.position = newPos;

        // Thuc hien di chuyen camera theo player voi offset
        Vector3 pos = _player.position + (Vector3)_camOffset;
        pos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = pos;
    }
}

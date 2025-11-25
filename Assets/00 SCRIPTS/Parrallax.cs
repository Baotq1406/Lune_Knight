using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrallax : MonoBehaviour
{
    [SerializeField]
    List<LoopImage> _loopingBGs = new List<LoopImage>();

    //[SerializeField]
    //List<float> speedsBG = new List<float>();

    [SerializeField] float _baseSpeed, _configSpeed;
    private Vector3 _lastPos;

    float _way;
    // Start is called before the first frame update
    void Start()
    {
        _lastPos = GameManager.Instance.Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // tinh toan chieu di chuyen
        float dx = GameManager.Instance.Player.transform.position.x - _lastPos.x;

        // khong di chuyen
        if (Mathf.Abs(dx) < 0.0001f)
            return;

        // cap nhat vi tri cuoi
        _lastPos = GameManager.Instance.Player.transform.position;

        // tinh toan huong di chuyen
        _way = GameManager.Instance.Player.transform.localScale.x < 0 ? -1 : 1;

        // neu nhan vat dang o trang thai dung yen hoac tan cong thi khong di chuyen background
        if (GameManager.Instance.Player.playerState == PlayerController.PlayerState.IDLE)
            return;

        // neu nhan vat dang o trang thai tan cong thi khong di chuyen background
        if (GameManager.Instance.Player.playerState == PlayerController.PlayerState.ATTACK)
            return;

        // di chuyen background
        for (int i = 0; i < _loopingBGs.Count; i++)
        {
            float speed = i * _baseSpeed * _configSpeed;
            _loopingBGs[i].transform.position += new Vector3(_way * speed * Time.deltaTime, 0, 0);
        }
    }
}

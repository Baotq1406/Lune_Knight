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

    float _way;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _way = GameManager.Instance.Player.transform.localScale.x < 0 ? -1 : 1;

        if (GameManager.Instance.Player.playerState == PlayerController.PlayerState.IDLE)
            return;

        if (GameManager.Instance.Player.playerState == PlayerController.PlayerState.ATTACK)
            return;

        for (int i = 0; i < _loopingBGs.Count; i++)
        {
            float speed = i * _baseSpeed * _configSpeed;
            _loopingBGs[i].transform.position += new Vector3(_way * speed * Time.deltaTime, 0, 0);
        }
    }
}

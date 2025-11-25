using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController; // su dung PlayerState tu PlayerController

public class AnimationControllerUnity : AnimationControllerBase
{
    Animator _animator; // component Animator cua nhan vat

    // Ham Start - khoi tao
    void Start()
    {
        _animator = this.GetComponent<Animator>(); // lay component Animator tren GameObject
    }

    // Ham cap nhat animation theo PlayerState
    public override void UpdateAnimation(PlayerState playerState)
    {
        // duyet tat ca cac trang thai PlayerState
        for (int i = 0; i <= (int)PlayerState.ATTACK; i++)
        {
            string stateName = ((PlayerState)i).ToString(); // lay ten string cua PlayerState

            if (playerState == ((PlayerState)i))
            {
                _animator.SetBool(stateName, true);  // set true cho trang thai hien tai
            }
            else
            {
                _animator.SetBool(stateName, false); // tat tat ca cac trang thai khac
            }
        }
    }
}

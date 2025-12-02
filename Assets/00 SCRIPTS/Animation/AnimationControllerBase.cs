using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;
using PlayerState = PlayerController.PlayerState; // dinh nghia PlayerState
using BossState = BossController.BossState; // dinh nghia BossState


public abstract class AnimationControllerBase : MonoBehaviour
{
    public abstract void UpdateAnimation(PlayerState playerState);

    public abstract void UpdateAnimationBoss(BossState bossState);
}

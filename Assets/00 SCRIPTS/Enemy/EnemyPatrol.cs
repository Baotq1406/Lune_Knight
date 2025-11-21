using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    #region Patrol Points
    [Header("Patrol Points")]
    [SerializeField] private Transform pointA; // diem trai
    [SerializeField] private Transform pointB; // diem phai
    #endregion

    #region Enemy
    [Header("Enemy")]
    [SerializeField] private Transform enemy; // transform cua enemy
    #endregion

    #region Movement Parameters
    [Header("Movement parameters")]
    [SerializeField] private float speed; // toc do di chuyen
    private Vector3 initScale; // luu scale ban dau de xoay
    private bool movingLeft; // huong di chuyen hien tai
    #endregion

    #region Idle Behaviour
    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration; // thoi gian dung idle
    private float idleTimer; // bo dem thoi gian dung
    #endregion

    #region Enemy Animator
    [Header("Enemy Animator")]
    [SerializeField] private Animator anim; // animator cua enemy
    #endregion


    private void Awake()
    {
        initScale = enemy.localScale; // luu scale ban dau de dung khi lat huong
    }

    private void OnDisable()
    {
        anim.SetBool("isRunning", false); // tat animation khi enemy bi vo hieu hoa
    }

    private void Update()
    {
        if (movingLeft)
        {
            // neu chua den diem trai thi di chuyen sang trai
            if (enemy.position.x >= pointA.position.x)
                MoveInDirection(-1);
            else
                DirectionChange(); // neu den diem trai thi doi huong
        }
        else
        {
            // neu chua den diem phai thi di chuyen sang phai
            if (enemy.position.x <= pointB.position.x)
                MoveInDirection(1);
            else
                DirectionChange(); // neu den diem phai thi doi huong
        }
    }

    private void DirectionChange()
    {
        anim.SetBool("isRunning", false); // tat animation khi dung
        idleTimer += Time.deltaTime; // dem thoi gian dung

        if (idleTimer > idleDuration)
            movingLeft = !movingLeft; // doi huong sau khi dung xong
    }

    private void MoveInDirection(int _direction)
    {
        idleTimer = 0; // reset bo dem khi bat dau di chuyen
        anim.SetBool("isRunning", true); // bat animation di chuyen

        // lat enemy de mat huong vao phuong di chuyen
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction,
            initScale.y, initScale.z);

        // di chuyen enemy theo phuong X
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
            enemy.position.y, enemy.position.z);
    }

    private void OnDrawGizmos()
    {
        // ve duong di chuyen va diem patrol tren editor
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointA.position, 0.5f); // diem trai
            Gizmos.DrawWireSphere(pointB.position, 0.5f); // diem phai
            Gizmos.DrawLine(pointA.position, pointB.position); // duong noi giua 2 diem
        }
    }
}

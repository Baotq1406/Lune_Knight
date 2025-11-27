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

    #region Chase Settings
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 4f; // tốc độ khi đuổi
    [SerializeField] private float detectionRange = 5f; // tầm nhìn raycast
    [SerializeField] private float stopDistance = 1f; // khoảng cách dừng lại khi đuổi player
    [SerializeField] private LayerMask playerLayer; // layer detect player
    [SerializeField] private Vector2 raycastOffset = new Vector2(0, 0.5f); // offset của raycast (x: ngang, y: dọc)

    private bool isChasing; // trạng thái đuổi theo player
    private Transform player; // transform của player khi phát hiện
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
        anim.SetBool(CONSTANT.IS_RUNNING, false); // tat animation khi enemy bi vo hieu hoa
    }

    private void Update()
    {
        if (isChasing)
        {
            // đuổi theo player
            ChasePlayer();
        }
        else
        {
            // phat hien player
            DetectPlayer();

            // nếu chưa phát hiện player → patrol
            PatrolBehaviour();
        }
    }

    private void DetectPlayer()
    {
        // hướng tia: -1 nếu trái, 1 nếu phải
        int dir = movingLeft ? -1 : 1; 
        
        // Tính vị trí gốc raycast với offset
        Vector2 origin = (Vector2)enemy.position + new Vector2(raycastOffset.x * dir, raycastOffset.y);
        Vector2 direction = new Vector2(dir, 0);// hướng raycast

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, detectionRange, playerLayer);

        Debug.DrawRay(origin, direction * detectionRange, hit.collider != null ? Color.green : Color.red);

        // nếu phát hiện player
        if (hit.collider != null)
        {
            isChasing = true;
            player = hit.collider.transform;
        }
    }
    
    // hàm đuổi theo player
    private void ChasePlayer()
    {
        if (player == null)
        {
            isChasing = false;
            return;
        }

        float distanceToPlayer = Vector2.Distance(enemy.position, player.position);

        // Nếu người chơi vượt quá phạm vi → ngừng đuổi
        if (distanceToPlayer > detectionRange + 1f)
        {
            isChasing = false;
            return;
        }

        // Nếu player quá gần → dừng di chuyển nhưng vẫn giữ animation
        if (distanceToPlayer <= stopDistance)
        {
            anim.SetBool(CONSTANT.IS_RUNNING, false);
            return; // không xoay, không di chuyển
        }

        anim.SetBool(CONSTANT.IS_RUNNING, true);

        // xác định hướng chạy tới player
        int direction = player.position.x < enemy.position.x ? -1 : 1;

        // Chỉ xoay mặt nếu hướng khác với hướng hiện tại
        float currentFacing = Mathf.Sign(enemy.localScale.x);
        if (currentFacing != direction)
        {
            enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * direction, initScale.y, initScale.z);
        }

        // di chuyển theo hướng player
        enemy.position += new Vector3(direction * chaseSpeed * Time.deltaTime, 0, 0);
    }

    // hàm di chuyển tuần tra
    private void PatrolBehaviour()
    {
        // di chuyển giữa 2 điểm patrol
        if (movingLeft)
        {
            // di chuyển về bên trái
            if (enemy.position.x >= pointA.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            // di chuyển về bên phải
            if (enemy.position.x <= pointB.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }

    private void DirectionChange()
    {
        anim.SetBool(CONSTANT.IS_RUNNING, false); // tat animation khi dung
        idleTimer += Time.deltaTime; // dem thoi gian dung

        if (idleTimer > idleDuration)
            movingLeft = !movingLeft; // doi huong sau khi dung xong
    }

    private void MoveInDirection(int _direction)
    {
        idleTimer = 0; // reset bo dem khi bat dau di chuyen
        anim.SetBool(CONSTANT.IS_RUNNING, true); // bat animation di chuyen

        // lat enemy de mat huong vao phuong di chuyen
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction,
            initScale.y, initScale.z);

        // di chuyen enemy theo phuong X
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
            enemy.position.y, enemy.position.z);
    }

    // Cập nhật hướng dựa trên scale hiện tại của enemy
    public void UpdateFacingDirection()
    {
        if (enemy == null) return;
        
        // Nếu scale.x âm → đang quay trái
        // Nếu scale.x dương → đang quay phải
        movingLeft = enemy.localScale.x < 0;
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

        // ve raycast phat hien player tren editor
        if (enemy != null) 
        { 
            int dir = movingLeft ? -1 : 1;  // huong raycast
            
            // Tính vị trí gốc raycast với offset (giống logic trong DetectPlayer)
            Vector3 origin = enemy.position + new Vector3(raycastOffset.x * dir, raycastOffset.y, 0);
            Vector3 endPoint = origin + new Vector3(dir * detectionRange, 0, 0);
            
            Gizmos.color = Color.yellow; // mau cua raycast
            Gizmos.DrawLine(origin, endPoint); // ve raycast
            
            // Vẽ điểm gốc để dễ thấy
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(origin, 0.1f);
        }
    }
}

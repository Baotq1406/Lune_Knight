using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    // 2 diem de enemy di chuyen giua
    public GameObject pointA;
    public GameObject pointB;

    // Rigidbody2D de dieu khien di chuyen bang vat ly
    private Rigidbody2D _rigi;

    // Animator de dieu khien animation
    [SerializeField] private Animator _anim;

    // Diem hien tai enemy di toi
    private Transform _currentPoint;

    // Toc do di chuyen
    public float speed = 3f;

    // Nguong de chuyen diem (de tranh rung)
    [SerializeField] private float _switchThresholdX = 0.05f;

    void Start()
    {
        _rigi = GetComponent<Rigidbody2D>(); // lay component Rigidbody2D
        // dat diem dich khoi tao (di toi pointB truoc)
        _currentPoint = pointB.transform;
        _anim.SetBool("isRunning", true); // bat animation chay
        UpdateFlip(); // lat sprite de huong mat vao target
    }

    void Update()
    {
        // tinh huong theo truc X toi target (1 = phai, -1 = trai)
        float dirX = Mathf.Sign(_currentPoint.position.x - transform.position.x);
        // ap dung van toc ngang, giu van toc doc
        _rigi.velocity = new Vector2(dirX * speed, _rigi.velocity.y);

        // neu den gan target, chuyen diem dich
        if (Mathf.Abs(transform.position.x - _currentPoint.position.x) <= _switchThresholdX)
        {
            // chuyen giua pointA va pointB
            _currentPoint = _currentPoint == pointB.transform ? pointA.transform : pointB.transform;
            UpdateFlip(); // lat sprite theo huong moi
        }
    }

    void UpdateFlip()
    {
        Vector3 scale = transform.localScale;
        // lat enemy de mat huong vao target
        // neu target o phai, lat phai (-scale.x); neu o trai, lat trai (+scale.x)
        scale.x = (_currentPoint.position.x > transform.position.x) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // Ve duong tu pointA den pointB tren editor de hieu duong di chuyen
    void OnDrawGizmosSelected()
    {
        if (pointA && pointB)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
        }
    }
}

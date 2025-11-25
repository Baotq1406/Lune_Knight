using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region CameraFollowSettings
    Transform _player;
    [SerializeField] Vector2 _camOffset;
    #endregion

    private Vector3 _shakeOffset = Vector3.zero;
    private Coroutine _shakeRoutine;

    void Start()
    {
        _player = GameManager.Instance.Player.transform;
    }

    void Update()
    {
        if (_player == null) return;

        Vector3 pos = _player.position + (Vector3)_camOffset + _shakeOffset;
        pos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = pos;
    }

    public void Shake(float duration, float magnitude)
    {
        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);
        _shakeRoutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float timer = 0f;
        while (timer < duration)
        {
            _shakeOffset = (Vector3)Random.insideUnitCircle * magnitude;
            timer += Time.deltaTime;
            yield return null;
        }
        _shakeOffset = Vector3.zero;
        _shakeRoutine = null;
    }
}
        
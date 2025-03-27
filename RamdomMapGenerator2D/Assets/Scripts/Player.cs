using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _canMoveAngle = 70f;
    [SerializeField] private float _speed = 4f;
    private CircleCollider2D _colider2D = null;
    private float _dirX = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _colider2D = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame

    private void Update()
    {
        if (IsGround() && CanMoveAngle())
        {
            _dirX = Input.GetAxis("Horizontal");

            transform.Translate(_dirX * _speed * Time.deltaTime, 0, 0);

            if (_dirX != 0f)
            {
                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Abs(newScale.x) * Mathf.Sign(_dirX); // 좌우 반전
                transform.localScale = newScale;
            }
        }
        else
        {
            // 공중일땐 수평 
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }


    private bool IsGround()
    {
        Vector2 center = (Vector2)_colider2D.bounds.center;

        Vector2 ray1 = new Vector2(center.x - _colider2D.radius, center.y);
        Vector2 ray2 = new Vector2(center.x + _colider2D.radius, center.y);
        Vector2 ray3 = center;

        Debug.DrawRay(ray1, -transform.up * 1.5f, Color.red);
        Debug.DrawRay(ray2, -transform.up * 1.5f, Color.red);
        Debug.DrawRay(ray3, -transform.up * 1.5f, Color.green);

        Vector2 direction = (Vector2)transform.up * -1f;

        RaycastHit2D leftHit = Physics2D.Raycast(ray1, direction, 1.5f, LayerMask.GetMask("Ground"));
        RaycastHit2D rightHit = Physics2D.Raycast(ray2, direction, 1.5f, LayerMask.GetMask("Ground"));
        RaycastHit2D middleHit = Physics2D.Raycast(ray3, direction, 1.5f, LayerMask.GetMask("Ground"));

        RaycastHit2D bestHit = middleHit.collider != null ? middleHit : (leftHit.collider != null ? leftHit : rightHit);

        if (bestHit.collider != null)
        {
            Vector2 normal = bestHit.normal; // 선택된 법선 벡터 사용
            float targetAngle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f; // 목표 회전 각도

            float currentAngle = transform.rotation.eulerAngles.z;
            if (currentAngle > 180f) currentAngle -= 360f; // 음수 각도 변환

            float minAngleChangeThreshold = 1f; // 너무 작은 각도 변화는 무시 (1도 이하)

            //  모서리에서 떨림 방지를 위해 일정 각도 이상 차이날 때만 회전
            if (Mathf.Abs(currentAngle - targetAngle) > minAngleChangeThreshold)
            {
                transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            }
        }
        return leftHit.collider != null || rightHit.collider != null;
    }


    private bool CanMoveAngle()
    {
        float zRotation = transform.rotation.eulerAngles.z;

        // 음수를 포함한 각도 계산
        if (zRotation > 180f)
        {
            zRotation -= 360f; // 음수 각도로 변환
        }

        if (Mathf.Abs(zRotation) <= _canMoveAngle)
            return true;

        return false;
    }
}

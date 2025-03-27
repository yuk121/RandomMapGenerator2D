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
        Vector2 ray3 = new Vector2(center.x - (_colider2D.radius / 2f), center.y);
        Vector2 ray4 = new Vector2(center.x + (_colider2D.radius / 2f), center.y);
        Vector2 ray5 = center;

        Debug.DrawRay(ray1, -transform.up * 0.9f, Color.red);
        Debug.DrawRay(ray2, -transform.up * 0.9f, Color.blue);
        Debug.DrawRay(ray3, -transform.up * 0.9f, Color.green);
        Debug.DrawRay(ray4, -transform.up * 0.9f, Color.yellow);
        Debug.DrawRay(ray5, -transform.up * 0.9f, Color.magenta);

        Vector2 direction = (Vector2)transform.up * -1f;

        RaycastHit2D leftHit = Physics2D.Raycast(ray1, direction, 0.9f, LayerMask.GetMask("Ground"));
        RaycastHit2D rightHit = Physics2D.Raycast(ray2, direction, 0.9f, LayerMask.GetMask("Ground"));
        RaycastHit2D middleLeftHit = Physics2D.Raycast(ray3, direction, 0.9f, LayerMask.GetMask("Ground"));
        RaycastHit2D middleRightHit = Physics2D.Raycast(ray4, direction, 0.9f, LayerMask.GetMask("Ground"));
        RaycastHit2D middle = Physics2D.Raycast(ray5, direction, 0.9f, LayerMask.GetMask("Ground"));


        // leftHit 또는 rightHit가 없을 경우 middleLeftHit 또는 middleRightHit를 대체
        if (leftHit.collider == null)
        {
            if (middleLeftHit.collider != null)
            {
                leftHit = middleLeftHit;
            }
            else
            {
                leftHit = middle;
            }
        }

        if (rightHit.collider == null)
        {
            if(middleRightHit.collider != null)
            {
                rightHit = middleRightHit;
            }
            else
            {
                rightHit = middle;
            }
        }

        if (leftHit.collider != null && rightHit.collider != null)
        {
            // 두 점의 차이를 이용해 수평 벡터 구함
            Vector2 green = leftHit.point - rightHit.point;

            // 90도 회전하여 법선 벡터 생성
            Vector2 normal = new Vector2(green.y, -green.x);
            normal.Normalize(); // 단위 벡터화

            float targetAngle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f; // 목표 회전 각도

            float currentAngle = transform.rotation.eulerAngles.z;
         
            if (currentAngle > 180f) 
                currentAngle -= 360f; // 음수 각도 변환

            float minAngleChangeThreshold = 1f; // 너무 작은 각도 변화는 무시 (1도 이하)

            // 목표 각도를 제한 각도까지
            float maxAngle = _canMoveAngle - 0.1f; // 제한 각도 (0 ~ _canMoveAngle)
            float minAngle = -_canMoveAngle + 0.1f; // 제한 각도의 음수 범위

            // 제한 각도 내에서만 회전하도록 조건 추가
            if (Mathf.Abs(currentAngle - targetAngle) > minAngleChangeThreshold)
            {
                // 제한 각도 초과 시 targetAngle 조정
                targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

                // 목표 각도로 회전
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

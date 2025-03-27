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
                newScale.x = Mathf.Abs(newScale.x) * Mathf.Sign(_dirX); // �¿� ����
                transform.localScale = newScale;
            }
        }
        else
        {
            // �����϶� ���� 
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
            Vector2 normal = bestHit.normal; // ���õ� ���� ���� ���
            float targetAngle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f; // ��ǥ ȸ�� ����

            float currentAngle = transform.rotation.eulerAngles.z;
            if (currentAngle > 180f) currentAngle -= 360f; // ���� ���� ��ȯ

            float minAngleChangeThreshold = 1f; // �ʹ� ���� ���� ��ȭ�� ���� (1�� ����)

            //  �𼭸����� ���� ������ ���� ���� ���� �̻� ���̳� ���� ȸ��
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

        // ������ ������ ���� ���
        if (zRotation > 180f)
        {
            zRotation -= 360f; // ���� ������ ��ȯ
        }

        if (Mathf.Abs(zRotation) <= _canMoveAngle)
            return true;

        return false;
    }
}

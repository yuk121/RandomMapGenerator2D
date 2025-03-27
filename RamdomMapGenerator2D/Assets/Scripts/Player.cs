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


        // leftHit �Ǵ� rightHit�� ���� ��� middleLeftHit �Ǵ� middleRightHit�� ��ü
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
            // �� ���� ���̸� �̿��� ���� ���� ����
            Vector2 green = leftHit.point - rightHit.point;

            // 90�� ȸ���Ͽ� ���� ���� ����
            Vector2 normal = new Vector2(green.y, -green.x);
            normal.Normalize(); // ���� ����ȭ

            float targetAngle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f; // ��ǥ ȸ�� ����

            float currentAngle = transform.rotation.eulerAngles.z;
         
            if (currentAngle > 180f) 
                currentAngle -= 360f; // ���� ���� ��ȯ

            float minAngleChangeThreshold = 1f; // �ʹ� ���� ���� ��ȭ�� ���� (1�� ����)

            // ��ǥ ������ ���� ��������
            float maxAngle = _canMoveAngle - 0.1f; // ���� ���� (0 ~ _canMoveAngle)
            float minAngle = -_canMoveAngle + 0.1f; // ���� ������ ���� ����

            // ���� ���� �������� ȸ���ϵ��� ���� �߰�
            if (Mathf.Abs(currentAngle - targetAngle) > minAngleChangeThreshold)
            {
                // ���� ���� �ʰ� �� targetAngle ����
                targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

                // ��ǥ ������ ȸ��
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

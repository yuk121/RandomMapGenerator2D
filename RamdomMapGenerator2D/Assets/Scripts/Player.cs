using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveAngleThreshold = 70f;
    [SerializeField] private float _speed = 4f;
    private Rigidbody2D _rb2D = null;
    private CircleCollider2D _colider2D = null;
    private float _dirX = 0;
    private bool _isMoveAngle = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _colider2D = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame

    private void Update()
    {
        if (IsGround())
        {
            _dirX = Input.GetAxis("Horizontal");

            //_rb2D.MovePosition(transform.position + new Vector3(_dirX * _speed * Time.deltaTime, 0));
            transform.Translate(_dirX * _speed * Time.deltaTime, 0, 0, Space.World);

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
        // CircleCollider2D �߽� ��������
        Vector2 center = (Vector2)_colider2D.bounds.center;

        // �׻� �ݶ��̴��� ���⿡ ���� ray ���� ��ġ�� ���
        Vector2 ray1 = center + (Vector2)(-transform.right * _colider2D.radius); // ���� ��
        Vector2 ray2 = center + (Vector2)(transform.right * _colider2D.radius);  // ������ ��

        float distance = 0.9f;// 0.75f;

        // �ð�ȭ 
        Debug.DrawRay(ray1, -transform.up * distance, Color.red);
        Debug.DrawRay(ray2, -transform.up * distance, Color.blue);

        // ����
        Vector2 direction = (Vector2)transform.up * -1f;

        // Raycast
        RaycastHit2D leftHit = Physics2D.Raycast(ray1, direction, distance, LayerMask.GetMask("Ground"));
        RaycastHit2D rightHit = Physics2D.Raycast(ray2, direction, distance, LayerMask.GetMask("Ground"));

        //Linecast 
        //RaycastHit2D area = Physics2D.Linecast(ray1 + direction * distance, ray2 + direction * distance, LayerMask.GetMask("Ground"));
        RaycastHit2D area = Physics2D.BoxCast(transform.position, new Vector2(_colider2D.radius * 2, _colider2D.radius * 2), 0f,direction, distance,LayerMask.GetMask("Ground"));

        Debug.DrawLine(ray1 + direction * distance, ray2 + direction * distance, Color.magenta);

        return leftHit.collider != null || rightHit.collider || area.collider != null; //hits.Length > 0f;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) 
            return; 

        Vector2 averageNormal = Vector2.zero;
        int contactCount = collision.contactCount;

        // ��� �浹 ������ ���� ���� �ջ�
        for (int i = 0; i < contactCount; i++)
        {
            averageNormal += collision.contacts[i].normal;
        }

        if (contactCount > 0)
        {
            averageNormal /= contactCount; // ��հ� ���
            averageNormal.Normalize(); // ���� ����ȭ

            // ��ǥ ȸ�� ���� ���
            float targetAngle = Mathf.Atan2(averageNormal.y, averageNormal.x) * Mathf.Rad2Deg - 90f;

            _isMoveAngle = CanMoveAngle(targetAngle);
 
            if (_isMoveAngle)
            {          
                transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            }
        }
    }

    // ������ �� �ִ� �������� Ȯ���ϴ� �Լ�
    private bool CanMoveAngle(float targetAngle)
    {
        // ��ǥ ������ ���� ������� ��ȯ
        if (targetAngle > 180f)
        {
            targetAngle -= 360f;
        }

        // ���� ���� ���� �ִ��� Ȯ��
        return Mathf.Abs(targetAngle) <= _moveAngleThreshold;
    }

    void OnDrawGizmos()
    {
        if(_colider2D != null)
        {
            Vector2 pos = new Vector2(transform.position.x + _colider2D.offset.x, transform.position.y + _colider2D.offset.y);
            Gizmos.DrawCube(pos, new Vector2(_colider2D.radius * 2, _colider2D.radius *2));
        }
    }
}

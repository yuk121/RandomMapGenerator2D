using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Body")]
    [SerializeField] private Transform _bodyTrans;
    [SerializeField] private float _moveAngleThreshold = 70f;
    [SerializeField] private float _speed = 4f;

    [Header("Artillery")]
    [SerializeField] private Transform _artilleryTrans;
    [SerializeField] private float _minAngleArtillery = 0f;
    [SerializeField] private float _maxAngleArtillery = 85f;

    private Rigidbody2D _rb2D = null;
    private CircleCollider2D _colider2D = null;
    private float _dirY = 0;
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
        _dirY = Input.GetAxis("Vertical");
        // 새로운 회전값 계산 (현재 회전 + 입력값)
       
        float currentZ = _artilleryTrans.eulerAngles.z;
        
        if (currentZ > 180f) 
            currentZ -= 360f; // 180° 이상이면 -180° ~ 0°로 변환

        float newZ = Mathf.Clamp(currentZ + _dirY * Time.deltaTime * 100f, _minAngleArtillery, _maxAngleArtillery);
        _artilleryTrans.rotation = Quaternion.Lerp(_artilleryTrans.rotation, Quaternion.Euler(0,0,newZ), Time.deltaTime * 10);

        // 포 각도 조절중 움직일 수 없음
        if (_dirY == 0 && IsGround())
        {
            _dirX = Input.GetAxis("Horizontal");

            //_rb2D.MovePosition(transform.position + new Vector3(_dirX * _speed * Time.deltaTime, 0));
            _bodyTrans.Translate(_dirX * _speed * Time.deltaTime, 0, 0, Space.World);

            if (_dirX != 0f)
            {
                Vector3 newScale = _bodyTrans.localScale;
                newScale.x = Mathf.Abs(newScale.x) * Mathf.Sign(_dirX); // 좌우 반전
                _bodyTrans.localScale = newScale;
            }
        }
        else
        {
            // 공중일땐 수평 
            _bodyTrans.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private bool IsGround()
    {
        // CircleCollider2D 중심 가져오기
        Vector2 center = (Vector2)_colider2D.bounds.center;

        // 항상 콜라이더의 기울기에 따라 ray 시작 위치를 계산
        Vector2 ray1 = center + (Vector2)(-_bodyTrans.right * _colider2D.radius); // 왼쪽 끝
        Vector2 ray2 = center + (Vector2)(_bodyTrans.right * _colider2D.radius);  // 오른쪽 끝

        float distance = 0.9f;// 0.75f;

        // 시각화 
        Debug.DrawRay(ray1, -_bodyTrans.up * distance, Color.red);
        Debug.DrawRay(ray2, -_bodyTrans.up * distance, Color.blue);

        // 방향
        Vector2 direction = (Vector2)transform.up * -1f;

        // Raycast
        RaycastHit2D leftHit = Physics2D.Raycast(ray1, direction, distance, LayerMask.GetMask("Ground"));
        RaycastHit2D rightHit = Physics2D.Raycast(ray2, direction, distance, LayerMask.GetMask("Ground"));

        //Linecast 
        //RaycastHit2D area = Physics2D.Linecast(ray1 + direction * distance, ray2 + direction * distance, LayerMask.GetMask("Ground"));
        RaycastHit2D area = Physics2D.BoxCast(_bodyTrans.position, new Vector2(_colider2D.radius * 2, _colider2D.radius * 2), 0f,direction, distance,LayerMask.GetMask("Ground"));

        Debug.DrawLine(ray1 + direction * distance, ray2 + direction * distance, Color.magenta);

        return leftHit.collider != null || rightHit.collider || area.collider != null; //hits.Length > 0f;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) 
            return; 

        Vector2 averageNormal = Vector2.zero;
        int contactCount = collision.contactCount;

        // 모든 충돌 지점의 법선 벡터 합산
        for (int i = 0; i < contactCount; i++)
        {
            averageNormal += collision.contacts[i].normal;
        }

        if (contactCount > 0)
        {
            averageNormal /= contactCount; // 평균값 계산
            averageNormal.Normalize(); // 단위 벡터화

            // 목표 회전 각도 계산
            float targetAngle = Mathf.Atan2(averageNormal.y, averageNormal.x) * Mathf.Rad2Deg - 90f;

            _isMoveAngle = CanMoveAngle(targetAngle);
 
            if (_isMoveAngle)
            {          
                transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            }
        }
    }

    // 움직일 수 있는 각도인지 확인하는 함수
    private bool CanMoveAngle(float targetAngle)
    {
        // 목표 각도도 같은 방식으로 변환
        if (targetAngle > 180f)
        {
            targetAngle -= 360f;
        }

        // 허용된 기울기 내에 있는지 확인
        return Mathf.Abs(targetAngle) <= _moveAngleThreshold;
    }

    void OnDrawGizmos()
    {
        if(_colider2D != null)
        {
            Vector2 pos = new Vector2(_bodyTrans.position.x + _colider2D.offset.x, _bodyTrans.position.y + _colider2D.offset.y);
            Gizmos.DrawCube(pos, new Vector2(_colider2D.radius * 2, _colider2D.radius *2));
        }
    }
}

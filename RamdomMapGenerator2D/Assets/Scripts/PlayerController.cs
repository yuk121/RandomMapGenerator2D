using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float MAX_POWER = 20f;
    private const float MIN_POWER = 0.1f;

    [Header("Body")]
    [SerializeField] private float _moveAngleThreshold = 70f;
    [SerializeField] private float _speed = 4f;

    [Header("Artillery")]
    [SerializeField] private Transform _artilleryTrans;
    [SerializeField] private Transform _bulletPos;
    [SerializeField] private float _artilleryRotateSpeed = 10f;
    [SerializeField] private float _minAngleArtillery = 0f;
    [SerializeField] private float _maxAngleArtillery = 85f;

    [Header("Shell")]
    [SerializeField] private List<GameObject> _shellList = new List<GameObject>();
    [SerializeField] private float _maxShellPower = 20f;
    [SerializeField] private float _minShellPower = 0.1f;
    [SerializeField] private float _curShellPower = 1f;
    [SerializeField] private float _powerGaugeSpeed = 1f;

    private Rigidbody2D _rb2D = null;               // Tank RigidBody 2D
    private CircleCollider2D _colider2D = null;     // Tank CircleCollider 2D
    private GameObject _curShell = null;            // ���� ���õ� ��ź    

    private float _dirY = 0;
    private float _dirX = 0;
    private bool _isMoveAngle = false;
    private float _curGauge = 0f;
    private float _prevShellPower = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    public void Init()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _colider2D = GetComponent<CircleCollider2D>();

        // Default
        _curShell = _shellList[0];
    }
    // Update is called once per frame

    private void Update()
    {
        // ��ź ����
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_shellList[0] == null)
            {
                Debug.LogWarning("Shell 1 is Null");
            }
            else
            {
                // 1�� ��ź
                _curShell = _shellList[0];
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_shellList[1] == null)
            {
                Debug.LogWarning("Shell 2 is Null");
            }
            else
            {
                // 2�� ��ź
                _curShell = _shellList[1];
            }
        }

        // �Ŀ������� ���� ��Ű��
        if (Input.GetKey(KeyCode.Space))
        {
            _curGauge += Time.deltaTime * _powerGaugeSpeed;
            _curShellPower = Mathf.Lerp(_minShellPower, _maxShellPower, _curGauge);
        }

        // ��ź �߻�
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _curGauge = 0f;
            Shell shell = GenerationShell();

            if (shell == null)
            {
                Debug.LogWarning("Generation Shell is Null !!!");
            }

            // �߻�
            shell.Fire(_curShellPower);

            // �߻��� ��ź ����
            _curShell = shell.gameObject;
            // ���� ��ź �Ŀ� �� ����
            _prevShellPower = _curShellPower;
        }

        _dirY = Input.GetAxis("Vertical");
        // ���ο� ȸ���� ��� (���� ȸ�� + �Է°�)

        float currentZ = _artilleryTrans.localEulerAngles.z;

        if (currentZ > 180f)
            currentZ -= 360f; // 180�� �̻��̸� -180�� ~ 0�Ʒ� ��ȯ

        float newZ = Mathf.Clamp(currentZ + _dirY * Time.deltaTime * _artilleryRotateSpeed, _minAngleArtillery, _maxAngleArtillery);
        Quaternion quaternion = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.eulerAngles.y, newZ);
        _artilleryTrans.localRotation = Quaternion.Lerp(_artilleryTrans.localRotation, quaternion, Time.deltaTime * 10);

        // �� ���� �ִ��� Ȯ��
        if (IsGround())
        {
            _dirX = Input.GetAxis("Horizontal");

            // �� ���� �����߿��� ������ �� ����
            if (_dirY == 0)
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

    private Shell GenerationShell()
    {
        GameObject go = PoolManager.Instance.Pop(_curShell.gameObject);

        if (go == null)
        {
            go = Instantiate(_curShell, _bulletPos.position, Quaternion.identity);
            PoolManager.Instance.Push(go);
        }
            
        go.transform.position = _bulletPos.position;
        go.transform.rotation = Quaternion.Euler(0, 0, _artilleryTrans.eulerAngles.z);
        
        // ��ź ���� 

        Vector3 newScale = go.transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * Mathf.Sign(transform.localScale.x); // �¿� ����
        go.transform.localScale = newScale;

        Shell shell = go.GetComponent<Shell>();

        switch(shell.ShellExplosionType)
        {
            case eShellExplosionType.Circle:
                shell.Init();
                return shell;
            
            case eShellExplosionType.Ellipse:
                ShellEllipse shellEllipse = go.GetComponent<ShellEllipse>();
                shellEllipse.Init();
                return shellEllipse;
        }
        return null;
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
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, targetAngle);
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
        //if(_colider2D != null)
        //{
        //    Vector2 pos = new Vector2(transform.position.x + _colider2D.offset.x, transform.position.y + _colider2D.offset.y);
        //    Gizmos.DrawCube(pos, new Vector2(_colider2D.radius * 2, _colider2D.radius *2));
        //}
    }
}

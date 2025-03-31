using UnityEngine;

public enum eShellExplosionType
{
    Circle,
    Ellipse,
}
public class Shell : MonoBehaviour
{
    [SerializeField] private eShellExplosionType _explosinType;
    public eShellExplosionType ShellExplosionType { get => _explosinType; }
    [SerializeField] private float _durtaion = 3.5f;        // 객체의 지속시간
    [SerializeField] protected float _radius = 2.5f;

    private Rigidbody2D _rb2D = null;
    protected BoxCollider2D _collider2D = null;

    private float _endTime = 0f;
    private float _power = 1f;
    private bool _isFire = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        _collider2D = GetComponent<BoxCollider2D>();
    }

    public void FixedUpdate()
    {
        if (_isFire)
        {
            _rb2D.AddForce(transform.position * _power);
            float angle = Mathf.Atan2(_rb2D.linearVelocity.y, _rb2D.linearVelocity.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0,0,angle);
        }

    }

    public void Update()
    {
        CheckExplosion();

        // 지속시간이 끝나면 없애기
        if (Time.time >= _endTime)
        {
            if (PoolManager.Instance != null)
            {
                PoolManager.Instance.Push(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void Init()
    {
        _rb2D = GetComponent<Rigidbody2D>();    
        _isFire = false;
        _endTime = Time.time + _durtaion;
    }

    public void Fire(float power)
    {
        _power = power;
        _isFire = true;
    }

    public virtual void CheckExplosion()
    {
        Debug.DrawRay(transform.position, Vector2.down * 0.5f, Color.magenta);

        RaycastHit2D hitPlayer = Physics2D.BoxCast(transform.position, _collider2D.size, transform.eulerAngles.z, Vector2.down, 0.5f, LayerMask.GetMask("Player"));
        RaycastHit2D hitGround = Physics2D.BoxCast(transform.position, _collider2D.size, transform.eulerAngles.z, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

        if (hitPlayer || hitGround)
        {
            if (hitPlayer.collider != null)
            {
                // 데미지 주기
            }

            Ground ground = hitGround.collider.GetComponent<Ground>();
            ground.GroundExplosion(transform.position, _radius);

            PoolManager.Instance.Push(gameObject);
        }
    }
}

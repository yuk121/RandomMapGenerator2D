using UnityEngine;

public enum eBombExplosionType
{
    Circle,
    Ellipse,
}
public class Bomb : MonoBehaviour
{
    [SerializeField] private eBombExplosionType _explosinType;
    [SerializeField] private float _durtaion = 3.5f;        // 객체의 지속시간
    [SerializeField] protected float _radius = 2.5f;

    private float _endTime = 0f;
    protected BoxCollider2D _collider2D = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        _collider2D = GetComponent<BoxCollider2D>();
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
        _endTime = Time.time + _durtaion;
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

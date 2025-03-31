using UnityEngine;

public class ShellEllipse : Shell
{
    [Header("Radius Y = Radius / RadiusY Ratio")]
    [SerializeField] private float _radiusYRatio = 2f;

    public override void CheckExplosion()
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
            float radiusY = _radius / _radiusYRatio;
            ground.GroundExplosion(transform.position, _radius, radiusY, eShellExplosionType.Ellipse);

            PoolManager.Instance.Push(gameObject);
        }
    }
}

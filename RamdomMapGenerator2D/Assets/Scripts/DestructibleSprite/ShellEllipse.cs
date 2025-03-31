using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ShellEllipse : Shell
{
    [Header("Radius Y = Radius / RadiusY Ratio")]
    [SerializeField] private float _radiusYRatio = 2f;

    public override void CheckExplosion()
    {
        Debug.DrawRay(transform.position, Vector2.down * 0.5f, Color.magenta);

        // BoxCollider2D�� �߽ɰ� ũ�⸦ ��������
        Vector2 colliderCenter = (Vector2)transform.position + _collider2D.offset;
        Vector2 colliderSize = _collider2D.size + (Vector2.one * 0.05f);

        // BoxCast�� ����Ͽ� ��ź�� �÷��̾� �Ǵ� ���� �浹 ����
        RaycastHit2D hit = Physics2D.BoxCast(colliderCenter, colliderSize, transform.eulerAngles.z, Vector2.down, 0f, LayerMask.GetMask("Player", "Ground"));

        // �浹 Ȯ�� �� 
        if (hit.collider != null)
        {
            // ���� �� ��� ���·� ���߹��� ��°��� ������
            float radiusY = _radius / _radiusYRatio;
            Collider2D[] hitAllPlayerList = Physics2D.OverlapCircleAll(colliderCenter, Mathf.Max(_radius, radiusY), LayerMask.GetMask("Player"));

            List<Collider2D> filteredPlayerList = new List<Collider2D>(hitAllPlayerList);

            // Ÿ�� �ۿ� �ִ� ��ü ����
            filteredPlayerList.RemoveAll(hitObject =>
            {
                Vector2 pos = hitObject.transform.position;
                float dx = (pos.x - colliderCenter.x) / _radius;
                float dy = (pos.y - colliderCenter.y) / radiusY;

                return (dx * dx) + (dy * dy) > 1;  // Ÿ�� ������ �˻翡 ������ ��� ����
            });

            Collider2D[] hitGroundList = Physics2D.OverlapCircleAll(colliderCenter, _radius, LayerMask.GetMask("Ground"));

            if (filteredPlayerList.Count > 0)
            {
                // ������ �ֱ�
            }

            if (hitGroundList.Length > 0)
            {
                foreach (var hitGround in hitGroundList)
                {
                    Ground ground = hitGround.GetComponent<Ground>();
                    ground.GroundExplosion(colliderCenter, _radius, radiusY, eShellExplosionType.Ellipse);
                }
            }

            // �浹�� ��쿡�� Pool
            PoolManager.Instance.Push(gameObject);
        }
    }
}

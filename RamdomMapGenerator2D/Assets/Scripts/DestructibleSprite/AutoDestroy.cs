using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float _durtaion = 3.5f;        // ��ü�� ���ӽð�
    private float _endTime = 0f;

    private void OnEnable()
    {
        _endTime = Time.time + _durtaion;
    }

    private void Update()
    {
        if (Time.time < _endTime)
            return;

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

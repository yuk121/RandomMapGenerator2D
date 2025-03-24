using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGenration : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _minStonrHeight;
    [SerializeField] private int _maxStonrHeight;
    [SerializeField] GameObject _dirt;
    [SerializeField] GameObject _grass;
    [SerializeField] GameObject _stone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Generation();        
    }

    private void Generation()
    {
        for (int x = 0; x < _width; x++)
        {
            int minHeight = _height - 1;
            int maxHeight = _height + 2;

            // 랜덤 높이 적용
            _height = Random.Range(minHeight, maxHeight);

            int minStoneSpawnDistance = _height - _minStonrHeight;
            int maxStoneSpawnDistance = _height - _maxStonrHeight;
            int totalStoneSpawnDistance = Random.Range(minStoneSpawnDistance, maxStoneSpawnDistance);

            for (int y = 0; y < _height; y++)
            {
                // 특정 높이 아래로는 Stone 생성
                if (y < totalStoneSpawnDistance)
                {
                  SpawnObj(_stone, x, y);
                }
                else
                {
                  SpawnObj(_dirt, x, y);
                }
            }

            // 맨 꼭대기에도 값에 따라 Stone 생성
            if (totalStoneSpawnDistance == _height)
            {
              SpawnObj(_stone, x, _height);
            }
            else
            {
              SpawnObj(_grass, x, _height);
            }
        }
    }

    private void SpawnObj(GameObject obj , int width , int height)
    {
        obj = Instantiate(obj, new Vector2(width, height), Quaternion.identity);
        obj.transform.parent = this.transform;
    }

}

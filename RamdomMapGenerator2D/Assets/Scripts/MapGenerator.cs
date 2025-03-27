using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private enum eMapType
    {
        None,
        Jungle,
        Desert,
        Ice,
        Max
    }

    [SerializeField] private List<GameObject> _mapPrefabList = new List<GameObject>();
    [SerializeField] private List<Texture2D> _mapGroundTextureList = new List<Texture2D>();
    [SerializeField] private Ground _mapGroundPrefab = null;

    [SerializeField] private float _randGroundScaleMin = 0.5f;        // �ּ� �� ũ��
    [SerializeField] private float _randGroundScaleMax = 1.5f;       // �ִ� �� ũ��

    [Range(0.1f, 1f)]
    [SerializeField] private float _groundUnderGenerationRatio;     // �� ���� ����

    [SerializeField] private float _randGroundIntervalMin = 1f;       // �ּ� �� ����
    [SerializeField] private float _randGroundIntervalMax = 3f;       // �ִ� �� ����


    private eMapType _mapType = eMapType.None;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomMapGeneration();
        RandomGroundGeneration();
    }

    public void RandomMapGeneration()
    {
        // ���� ����
        _mapType =(eMapType)Random.Range((int)eMapType.Jungle, (int)eMapType.Max);

        // ��� ����
        GameObject map = Instantiate(_mapPrefabList[(int)_mapType]);
        map.transform.parent = this.transform;
    }

    private void RandomGroundGeneration()
    {
        // ���� �ϴ� �����ϱ�
        // ���� ī�޶� ������ ���� ��ǥ�� �ϱ�
        float height = Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;

        // �� ���� ���� (-width~width), (-height~0)
   
       
        // �� ����
        Ground ground = Instantiate(_mapGroundPrefab);
        ground.transform.parent = this.transform;
        ground.Init(_mapGroundTextureList[(int)_mapType]);

        // ���� ũ�� �ο�
        float randScale = Random.Range(_randGroundScaleMin, _randGroundScaleMax);
        ground.transform.localScale = ground.transform.localScale * randScale;

        // ���� �� ����
        float randInterval = Random.Range(_randGroundIntervalMin, _randGroundIntervalMax);
    }

    //public float MakeNoise(int width, int height)
    //{
    //    float amplitude = _amplitude;                             // ����
    //    float frequnecy = noiseSettings.startFrequency;      // ��
    //    float noiseSum = 0;
    //    float amplitudeSum = 0;

    //    for (int i = 0; i < noiseSettings.octaves; i++)
    //    {
    //        noiseSum += amplitude * Mathf.PerlinNoise(x * frequnecy, y * frequnecy);
    //        amplitudeSum += amplitude;
    //        amplitude *= noiseSettings.persistance;
    //        frequnecy *= noiseSettings.frequencyModifier;
    //    }

    //    float nomalize = noiseSum / amplitudeSum; // [0 - 1]

    //    return nomalize;
    //}
}

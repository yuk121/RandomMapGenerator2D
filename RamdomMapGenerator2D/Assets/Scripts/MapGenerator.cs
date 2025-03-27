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

    [SerializeField] private float _randGroundScaleMin = 0.5f;        // 최소 땅 크기
    [SerializeField] private float _randGroundScaleMax = 1.5f;       // 최대 땅 크기

    [Range(0.1f, 1f)]
    [SerializeField] private float _groundUnderGenerationRatio;     // 땅 생성 비율

    [SerializeField] private float _randGroundIntervalMin = 1f;       // 최소 땅 간격
    [SerializeField] private float _randGroundIntervalMax = 3f;       // 최대 땅 간격


    private eMapType _mapType = eMapType.None;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomMapGeneration();
        RandomGroundGeneration();
    }

    public void RandomMapGeneration()
    {
        // 랜덤 선택
        _mapType =(eMapType)Random.Range((int)eMapType.Jungle, (int)eMapType.Max);

        // 배경 생성
        GameObject map = Instantiate(_mapPrefabList[(int)_mapType]);
        map.transform.parent = this.transform;
    }

    private void RandomGroundGeneration()
    {
        // 땅과 하늘 구분하기
        // 현재 카메라 시점의 월드 좌표로 하기
        float height = Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;

        // 땅 생성 범위 (-width~width), (-height~0)
   
       
        // 땅 생성
        Ground ground = Instantiate(_mapGroundPrefab);
        ground.transform.parent = this.transform;
        ground.Init(_mapGroundTextureList[(int)_mapType]);

        // 랜덤 크기 부여
        float randScale = Random.Range(_randGroundScaleMin, _randGroundScaleMax);
        ground.transform.localScale = ground.transform.localScale * randScale;

        // 랜덤 땅 간격
        float randInterval = Random.Range(_randGroundIntervalMin, _randGroundIntervalMax);
    }

    //public float MakeNoise(int width, int height)
    //{
    //    float amplitude = _amplitude;                             // 진폭
    //    float frequnecy = noiseSettings.startFrequency;      // 빈도
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

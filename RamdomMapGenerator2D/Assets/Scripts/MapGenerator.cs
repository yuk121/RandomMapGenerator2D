using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapRenderer _mapRenderer;

    [Header("Map")]
    [SerializeField] private int _mapLength = 0;           // 길이
    [SerializeField] private float _amplitude = 1;          // 진폭
    [SerializeField] private float _frequency = 0.01f;      // 빈도

    [Header("Tile")]
    [SerializeField] private TileBase _tileDirt = null;
    [SerializeField] private TileBase _tileDirtGrass = null;
    [SerializeField] private TileBase _tileStone = null;
    [SerializeField] private TileBase _tileStoneGrass = null;
    [SerializeField] private TileBase _tilePerlin2D = null;

    [SerializeField] private float _noiseThresholdMin = 0.45f;
    [SerializeField] private float _noiseThresholdMax = 0.55f;

    [Header("Data")]
    [SerializeField] private NoiseDataS0 _heightMapNoiseData;
    [SerializeField] private NoiseDataS0 _stoneNoiseData;
    [SerializeField] private NoiseDataS0 _perlin2DData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerareMap();
    }

    [ContextMenu("Map Generation")]
    // 노이즈 선 만드는 메소드
    public void GenerareMap()
    {
        _mapRenderer.ClearGroundTileMap();
        for (int x = 0; x < _mapLength; x++)
        {
            //Dirt
            var noise = SumNoise(_heightMapNoiseData.offset.x + x, 1, _heightMapNoiseData);
            var noiseInRange = RangeMap(noise, 0, 1, _heightMapNoiseData.noiseRangeMin, _heightMapNoiseData.noiseRangeMax);

            var noiseEndValue = Mathf.FloorToInt(noiseInRange);

            // Stone
            var noiseStone = SumNoise(_stoneNoiseData.offset.x + x, 1, _stoneNoiseData);
            var noiseStoneInRange = RangeMap(noiseStone, 0, 1, _stoneNoiseData.noiseRangeMin, _stoneNoiseData.noiseRangeMax);

            var noiseStoneInt = Mathf.FloorToInt(noiseStoneInRange);

            for (int y = 0; y <= noiseEndValue; y++)
            {
                // Perlin
                var noisePerlin2D = SumNoise(_perlin2DData.offset.x + x, y, _perlin2DData);
                if (y >= _perlin2DData.noiseRangeMin && y <= _perlin2DData.noiseRangeMax && 
                    noisePerlin2D > _noiseThresholdMin && noisePerlin2D < _noiseThresholdMax)
                {
                    //_mapRenderer.SetPerlin2D(x, y, _tilePerlin2D);
                    continue;
                }

                TileBase selectTile = SelectTile(y, noiseEndValue, noiseStoneInt);
                _mapRenderer.SetGroundTile(x,y, selectTile);
            }
        }
    }

    [ContextMenu("Generate Perlin2D")]
    public void GenerateMapPerlin2D()
    {
        _mapRenderer.ClearPerlin2DTileMap();

        for (int x = -1 * _mapLength; x < _mapLength; x++)
        {
            for (int y = _perlin2DData.noiseRangeMin; y < _perlin2DData.noiseRangeMax; y++)
            {
                var noise = SumNoise(_perlin2DData.offset.x + x, y, _perlin2DData);
                if (noise > _noiseThresholdMin && noise < _noiseThresholdMax)
                {
                    _mapRenderer.SetPerlin2D(x, y, _tilePerlin2D);
                }
            }
        }
    }

    [ContextMenu("Map Clear")]
    private void MapClear()
    {
        _mapRenderer.ClearGroundTileMap();
        _mapRenderer.ClearPerlin2DTileMap();
    }

    private TileBase SelectTile(int y, int noiseEndValue, int stoneHeight)
    {
        if(y >= stoneHeight)
        {
            // 맨 꼭대기
            if (y == noiseEndValue)
                return _tileStoneGrass;

            return _tileStone;
        }
        else if(y == noiseEndValue)
        {
            return _tileDirtGrass;
        }

        return _tileDirt;
    }

    public float SumNoise(int x, int y , NoiseDataS0 noiseSettings)
    {
        float amplitude = 1f;
        float frequnecy = noiseSettings.startFrequency;
        float noiseSum = 0;
        float amplitudeSum = 0;

        for(int i =0; i < noiseSettings.octaves; i++)
        {
            noiseSum += amplitude * Mathf.PerlinNoise(x * frequnecy, y * frequnecy);
            amplitudeSum += amplitude;
            amplitude *= noiseSettings.persistance;
            frequnecy *= noiseSettings.frequencyModifier;
        }

        float nomalize = noiseSum / amplitudeSum; // [0 - 1]

        return nomalize;
    }

    private float RangeMap(float inputValue, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (inputValue - inMin) * (outMax - outMin) / (inMax - inMin);
    }
   
}

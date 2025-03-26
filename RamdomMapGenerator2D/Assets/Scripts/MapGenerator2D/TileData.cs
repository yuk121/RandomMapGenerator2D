using NUnit.Framework;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileData : MonoBehaviour
{
    [SerializeField] TileBase _dirt;
    [SerializeField] TileBase _dirtGrass;
    [SerializeField] TileBase _stoneGrass;
    [SerializeField] TileBase _stone;
    [SerializeField] TileBase _perlin2D;
}

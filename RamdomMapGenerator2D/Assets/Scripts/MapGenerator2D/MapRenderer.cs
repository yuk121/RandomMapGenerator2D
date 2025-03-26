using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private Tilemap _backgroundTileMap;
    [SerializeField] private Tilemap _groundTileMap;
    [SerializeField] private Tilemap _perlind2DTileMap;

    public void SetPerlin2D(int x, int y , TileBase tile)
    {
        _perlind2DTileMap.SetTile(_perlind2DTileMap.WorldToCell(new Vector3Int(x, y, 0)), tile);
    }

    public void SetGroundTile(int x, int y, TileBase tile)
    {
        _groundTileMap.SetTile(_groundTileMap.WorldToCell(new Vector3Int(x, y, 0)), tile);
    }
    
    public void SetBackgroundTile(int x, int y, TileBase tile)
    {
        _backgroundTileMap.SetTile(_backgroundTileMap.WorldToCell(new Vector3Int(x,y,0)), tile);    
    }

    public void ClearGroundTileMap()
    {
        _groundTileMap.ClearAllTiles();
        _backgroundTileMap.ClearAllTiles();
    }

    public void ClearPerlin2DTileMap()
    {
        _perlind2DTileMap.ClearAllTiles();
    }
}



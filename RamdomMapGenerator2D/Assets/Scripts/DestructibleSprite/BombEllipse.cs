using UnityEngine;

public class BombEllipse : Bomb
{
    [Header("Radius Y = Radius X / RadiusY Ratio")]
    [SerializeField] private float _radiusX = 3f;
    public float RadiusX { get => _radiusX; }
    [SerializeField] private float _radiusYRatio = 2f;
    public float RadiusYRatio { get => _radiusYRatio; }
}

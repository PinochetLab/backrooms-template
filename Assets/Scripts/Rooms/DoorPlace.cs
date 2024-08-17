using UnityEngine;

public class DoorPlace
{
    public static readonly float Width = 0.2f;
    public static readonly float WallThickness = 0.1f;
    public Vector2 Position { get; }
    public bool Horizontal { get; }

    public DoorPlace(Vector2 position, bool horizontal)
    {
        Horizontal = horizontal;
        Position = position;
    }

    public void Draw()
    {
        var dir = Horizontal ? Vector2.right : Vector2.up;
        var delta = dir * Width / 2;
        var p0 = Position - delta;
        var p1 = Position + delta;
        Gizmos.DrawLine(p0, p1);
    }
}

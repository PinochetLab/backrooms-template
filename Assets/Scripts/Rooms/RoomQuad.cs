using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomQuad : Quad
{
    private List<float>[] _cutouts;

    public RoomQuad(Vector2 position, Vector2 size) : base(position, size)
    {
        _cutouts = Enumerable.Range(0, 4).Select(_=> new List<float>()).ToArray();
    }

    public void AddCutout(int wallIdx, float pos)
    {
        _cutouts[wallIdx].Add(pos);
    }

    public Tuple<float, float> GetStartAndEnd(bool horizontal)
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (horizontal)
        {
            return new Tuple<float, float>(Position.x - Size.x / 2, Position.x + Size.x / 2);
        }
        {
            return new Tuple<float, float>(Position.y - Size.y / 2, Position.y + Size.y / 2);
        }
    }
    
    public void Draw()
    {
        var size = Size - Vector2.one * DoorPlace.WallThickness;
        var p1 = Position + size / 2;
        var p3 = Position - size / 2;
        var p0 = new Vector2(p1.x, p3.y);
        var p2 = new Vector2(p3.x, p1.y);
        var points = new List<Vector3> { p0, p1, p2, p3 };
        var pointsRoS = new ReadOnlySpan<Vector3>(points.ToArray());
        Gizmos.DrawLineStrip(pointsRoS, true);
        for (var i = 0; i < 4; i++)
        {
            var nextPoint = points[(i + 1) % 4];
            var dir = (nextPoint - points[i]).normalized;
            foreach (var cutout in _cutouts[i])
            {
                Debug.Log(Position + " " + points[i] + " " + dir);
                var point = points[i] + dir * (cutout - DoorPlace.WallThickness / 2);
                Gizmos.DrawCube(point, Vector3.one * DoorPlace.Width);
            }
        }
    }
}

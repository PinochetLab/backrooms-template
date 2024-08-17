using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Quad
{
    private class Event : IComparable<Event>
    {
        public float Pos { get; }
        public bool Close { get; }
        public bool Top { get; }
        public RoomQuad RoomQuad { get;}

        public Event(float pos, bool close, bool top, RoomQuad roomQuad)
        {
            Pos = pos;
            Close = close;
            Top = top;
            RoomQuad = roomQuad;
        }

        public int CompareTo(Event other)
        {
            const float delta = 0.001f;
            if (Mathf.Abs(Pos - other.Pos) > delta) return Pos.CompareTo(other.Pos);
            return -Close.CompareTo(other.Close);
        }
    }
    
    protected readonly Vector2 Position;
    protected readonly Vector2 Size;

    protected Quad()
    {
        Position = Vector2.zero;
        Size = Vector2.one;
    }

    public Quad(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public DivideAnswer Divide(bool horizontal, int depth = 6)
    {
        if (depth == 0)
        {
            var roomQuad = new RoomQuad(Position, Size);
            var lst = new List<RoomQuad>{ roomQuad };
            var edgeQuads = Enumerable.Repeat(lst, 4).ToArray();
            return new DivideAnswer(lst, edgeQuads, new List<DoorPlace>());
        }
        const float minRatio = 0.35f;
        var ratio = Random.Range(minRatio, 1 - minRatio);
        var lDir = horizontal ? Vector2.right : Vector2.up;
        var tDir = horizontal ? Vector2.up : Vector2.right;
        var sliceDir = horizontal ? Vector2.right : Vector2.up;
        var lSize = Mathf.Abs(Vector2.Dot(Size, lDir));
        var tSize = Mathf.Abs(Vector2.Dot(Size, tDir));
        var tSize1 = tSize * ratio;
        var tSize2 = tSize * (1 - ratio);
        var size1 = lDir * lSize + tDir * tSize1;
        var size2 = lDir * lSize + tDir * tSize2;
        var pos1 = Position - tDir * ((tSize - tSize1) / 2);
        var pos2 = Position + tDir * ((tSize - tSize2) / 2);
        var quad1 = new Quad(pos1, size1);
        var quad2 = new Quad(pos2, size2);
        var answer1 = quad1.Divide(!horizontal, depth - 1);
        var answer2 = quad2.Divide(!horizontal, depth - 1);
        var edgeQuads0 = horizontal ? Concat(answer1.EdgeQuads[0], answer2.EdgeQuads[0]) : answer2.EdgeQuads[0];
        var edgeQuads1 = horizontal ? answer2.EdgeQuads[1] : Concat(answer1.EdgeQuads[1], answer2.EdgeQuads[1]);
        var edgeQuads2 = horizontal ? Concat(answer1.EdgeQuads[2], answer2.EdgeQuads[2]) : answer1.EdgeQuads[2];
        var edgeQuads3 = horizontal ? answer1.EdgeQuads[3] : Concat(answer1.EdgeQuads[3], answer2.EdgeQuads[3]);
        var doorPlaces = answer1.DoorPlaces.Concat(answer2.DoorPlaces).ToList();

        var topQuads = horizontal ? answer2.EdgeQuads[3] : answer1.EdgeQuads[0];
        var bottomQuads = horizontal ? answer1.EdgeQuads[1] : answer2.EdgeQuads[2];
        
        var events = new List<Event>();
        foreach (var quad in topQuads)
        {
            var startAndEnd = quad.GetStartAndEnd(horizontal);
            events.Add(new Event(startAndEnd.Item1, false, true, quad));
            events.Add(new Event(startAndEnd.Item2, true, true, quad));
        }
        foreach (var quad in bottomQuads)
        {
            var startAndEnd = quad.GetStartAndEnd(horizontal);
            events.Add(new Event(startAndEnd.Item1, false, false, quad));
            events.Add(new Event(startAndEnd.Item2, true, false, quad));
        }
        events.Sort();

        var tSlicePos = Vector2.Dot(tDir, Position) - tSize / 2 + tSize1;
        if (depth == 6) Debug.Log(topQuads.Count + ", " + bottomQuads.Count);

        RoomQuad topQuad = null;
        RoomQuad bottomQuad = null;
        float lastPos = 0;
        foreach (var roomEvent in events)
        {
            if (depth == 6) Debug.Log("(" + roomEvent.Pos + ", " + (roomEvent.Close ? "closed" : "opened") + ", " + (roomEvent.Top ? "top" : "bottom") + ", last: " + lastPos + ")");
            if (roomEvent.Close)
            {
                if (topQuad != null && bottomQuad != null)
                {
                    var pos = roomEvent.Pos;
                    var delta = pos - lastPos;
                    var maxWidth = DoorPlace.Width + DoorPlace.WallThickness;
                    if (delta > maxWidth)
                    {
                        var center = Random.Range(lastPos + maxWidth / 2, pos - maxWidth / 2);
                        var doorPos = tDir * tSlicePos + lDir * center;
                        var doorPlace = new DoorPlace(doorPos, horizontal);
                        doorPlaces.Add(doorPlace);
                        if (horizontal)
                        {
                            topQuad.AddCutout(3, center - topQuad.GetStartAndEnd(true).Item1);
                            bottomQuad.AddCutout(1, bottomQuad.GetStartAndEnd(true).Item2 - center);
                        }
                        else
                        {
                            topQuad.AddCutout(0, center - topQuad.GetStartAndEnd(false).Item1);
                            bottomQuad.AddCutout(2, bottomQuad.GetStartAndEnd(false).Item2 - center);
                        }
                    }
                }
                if (roomEvent.Top) topQuad = null;
                else bottomQuad = null;
            }
            else
            {
                lastPos = roomEvent.Pos;
                if (roomEvent.Top) topQuad = roomEvent.RoomQuad;
                else bottomQuad = roomEvent.RoomQuad;
            }
        }
        
        var answer = new DivideAnswer(
            Concat(answer1.Quads, answer2.Quads),
            new [] { edgeQuads0, edgeQuads1, edgeQuads2, edgeQuads3 },
            doorPlaces
        );
        
        return answer;

        List<RoomQuad> Concat(List<RoomQuad> q1, List<RoomQuad> q2) => q1.Concat(q2).ToList();
    }
}

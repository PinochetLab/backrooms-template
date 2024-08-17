using System;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    private List<RoomQuad> _quads = new ();
    private List<DoorPlace> _doorPlaces = new ();

    private void Awake()
    {
        BuildMap();
    }

    private void BuildMap()
    {
        var mainQuad = new Quad(Vector2.zero, Vector2.one * 10);
        var divideAnswer = mainQuad.Divide(false);
        _quads = divideAnswer.Quads;
        _doorPlaces = divideAnswer.DoorPlaces;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        _quads.ForEach(quad => quad.Draw());
        Gizmos.color = Color.green;
        _doorPlaces.ForEach(quad => quad.Draw());
    }
}

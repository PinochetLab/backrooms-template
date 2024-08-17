using System.Collections.Generic;

public class DivideAnswer
{
    public List<RoomQuad> Quads { get; }
    public List<RoomQuad>[] EdgeQuads { get; }
    
    public List<DoorPlace> DoorPlaces { get; }

    public DivideAnswer(List<RoomQuad> quads, List<RoomQuad>[] edgeQuads, List<DoorPlace> doorPlaces)
    {
        Quads = quads;
        EdgeQuads = edgeQuads;
        DoorPlaces = doorPlaces;
    }
}

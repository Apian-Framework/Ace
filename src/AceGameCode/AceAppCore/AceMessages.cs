using Apian;

namespace AceGameCode
{
    public class AceMessage : ApianCoreMessage
    {
        public const string kPlacePlane = "PlPl";
        public const string kMovePlane = "MvPl";

        public AceMessage(string type) : base(type, 0) {} // TODO: Does Ace care about timestamps?

    }

    public class PlacePlaneMsg : AceMessage
    {
        public string planeId;
        public int xPos;
        public int yPos;
        public string orientationHash;
        public PlacePlaneMsg() : base(kPlacePlane)  {}
        public PlacePlaneMsg(string _planeId, int _x, int _y, string _orHash ) : base(kPlacePlane)
        {
            planeId = _planeId;
            xPos = _x;
            yPos = _y;
            orientationHash = _orHash;
        }
    }

    public class MovePlaneMsg : AceMessage
    {
        public string planeId;
        public PlaneOrientation orient;
        public int spaces;
        public string newOrientHash;

        public MovePlaneMsg() : base(kPlacePlane)  {}

        public MovePlaneMsg(string _planeId, PlaneOrientation _orient, int _spaces, string _newOrHash ) : base(kPlacePlane)
        {
            planeId = _planeId;
            orient = _orient;
            spaces = _spaces;
            newOrientHash = _newOrHash;
        }

    }


}
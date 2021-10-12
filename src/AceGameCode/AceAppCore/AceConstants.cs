namespace AceGameCode
{
    public enum AcePeerRole : int
    {
        kNone = 0,
        kPlayer = 1,
        kValidator = 2,
        kPreferPlayer= 3, // Player if available, if not then validator.
    }
    public enum PlaneColor : int
    {
        kNone = 0, // None for "I don't care"
        kRed = 1,
        kBlue = 2,

    }
    public enum BoardSide
    {
        kNorth = 0,
        kSouth =  1
    }

    public enum PlaneOrientation
    {
        // From the North side of the board looking thru it South, East is to the left.
        kUp = 0,
        kUpWest = 1,
        kWest = 2,
        kDownWest = 3,
        kDown = 4,
        kDownEast = 5,
        kEast = 6,
        kUpEast = 7,
        kConcealed = 8
    }
}
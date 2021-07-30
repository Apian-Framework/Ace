namespace AceGameCode
{
    public enum PlayerRole : int
    {
        kNone = 0,
        kPlayer = 1,
        kValidator = 2,
    }
    public enum PlaneColor : int
    {
        kNone = 0,
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
        // From the plane's side of the board, clockwise
        kUp = 0,
        kUpRight = 1,
        kRight = 2,
        kDownRight=3,
        kDown = 4,
        kDownLeft = 5,
        kLeft = 6,
        kUpLeft = 7,
        kConcealed = 8
    }
}
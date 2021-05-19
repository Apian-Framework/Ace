namespace AceGameCode
{
    public class AceBoard
    {

        public const int DefaultWidth = 6;
        public const int DefaultHeight = 5;
        public int Width {get; private set;}
        public int Height {get; private set;}
        public AcePlane[,] spaces;

        public AceBoard(int width = DefaultWidth, int height = DefaultHeight)
        {
            Width = width;
            Height = height;
            spaces = new AcePlane[width, height];
        }

    }
}
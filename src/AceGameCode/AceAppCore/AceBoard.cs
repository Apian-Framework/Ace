using Newtonsoft.Json;
using Apian;

namespace AceGameCode
{
    public class AceBoard : IApianCoreData
    {

        public const int DefaultWidth = 6;
        public const int DefaultHeight = 5;
        public int Width {get; private set;}
        public int Height {get; private set;}

        public AceBoard(int width = DefaultWidth, int height = DefaultHeight)
        {
            Width = width;
            Height = height;
        }

        // IApianCoreData
        // Custom compact json
        // TODO: set up params to make more compact.
        public static AceBoard FromApianJson(string jsonData)
        {
            object[] data = JsonConvert.DeserializeObject<object[]>(jsonData);
            return new AceBoard(
                (int)(long)data[0],
                (int)(long)data[1]
            );
        }

        public string ApianSerialized(object args=null)
        {
            return  JsonConvert.SerializeObject(new object[]{
                Width,
                Height
             });
        }

    }
}
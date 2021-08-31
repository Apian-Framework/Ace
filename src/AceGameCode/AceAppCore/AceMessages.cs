using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Apian;

namespace AceGameCode
{
    public class AceMessage : ApianCoreMessage
    {
        public const string kNewPlayer = "AcNPlr";
        public const string kPlayerLeft = "AcPlrLft";
        public const string kNewGame = "AcNwGm";
        public const string kPlacePlane = "AcPlPl";
        public const string kMovePlane = "AcMvPl";
        public const string kTakePlane = "AcTkPl";
        public const string kDogfight = "AcDgft";
        public const string kGameOver = "AcGmOv";

        public AceMessage(string type, long timeStamp) : base(type, timeStamp) {}
        public AceMessage() : base() {} // Must have this for Newtonsoft
    }

    public class NewPlayerMsg : AceMessage
    {
        // When initially sent as a request, Role and team are
        // the *desired* role and team - they may not be what you get.
        // In the case of "role" if you request one and don't get what you want
        // then you leave the game.
        // TODO: make this work
        public AcePlayer newPlayer;
        public NewPlayerMsg(long ts, AcePlayer _newPlayer) : base(kNewPlayer, ts) => newPlayer = _newPlayer;
        public NewPlayerMsg() : base() {}
    }

    public class PlayerLeftMsg : AceMessage
    {
        public string peerId;
        public PlayerLeftMsg(long ts, string _peerId) : base(kPlayerLeft, ts) => peerId = _peerId;
        public PlayerLeftMsg() : base() {}

    }

    // public class NewGameMsg : AceMessage
    // {
    //     public string PlayerId;
    //     public PlayerRole DesiredRole;
    //     public PlaneColor DesiredTeam;

    //     public NewGameMsg(long ts, string playerId) : base(kNewGame, ts) => PlayerId = _peerId;
    //     public NewGameMsg() : base() {}

    // }

    public class PlacePlaneMsg : AceMessage
    {
        public string planeId;
        public int xPos;
        public int yPos;
        public string orientationHash;
        public PlacePlaneMsg() : base()  {}
        public PlacePlaneMsg(long ts, string _planeId, int _x, int _y, string _orHash ) : base(kPlacePlane, ts)
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

        public MovePlaneMsg() : base()  {}

        public MovePlaneMsg(long ts, string _planeId, PlaneOrientation _orient, int _spaces, string _newOrHash ) : base(kPlacePlane, ts)
        {
            planeId = _planeId;
            orient = _orient;
            spaces = _spaces;
            newOrientHash = _newOrHash;
        }

    }

    // Serialization

    static public class AceCoreMessageDeserializer
    {

         public static Dictionary<string, Func<string, ApianCoreMessage>> aceDeserializers = new  Dictionary<string, Func<string, ApianCoreMessage>>()
         {
            {AceMessage.kNewPlayer, (s) => JsonConvert.DeserializeObject<NewPlayerMsg>(s) },

            {AceMessage.kPlayerLeft, (s) => JsonConvert.DeserializeObject<PlayerLeftMsg>(s) },
            {AceMessage.kPlacePlane, (s) => JsonConvert.DeserializeObject<PlacePlaneMsg>(s) },
            {AceMessage.kMovePlane, (s) => JsonConvert.DeserializeObject<MovePlaneMsg>(s) },

            // This is a sort-of generic Apian-defined CoreMessage. In practice the Player-related
            // messages maybe outo to go there as well.
            // Also - these entries probably ought to be in an Apian-defined dict that is checks along with the local one.
            // ...or maybe merge them... I dunno.
            {ApianMessage.CheckpointMsg, (s) => JsonConvert.DeserializeObject<ApianCheckpointMsg>(s) },
         };

        public static ApianCoreMessage FromJSON(string coreMsgType, string json)
        {
            return  aceDeserializers[coreMsgType](json) as ApianCoreMessage;
        }
    }

}
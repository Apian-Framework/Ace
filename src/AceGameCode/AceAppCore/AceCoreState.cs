using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Apian;
using UniLog;
using static UniLog.UniLogger; // for SID()

namespace AceGameCode
{
    public class AceCoreState : ApianCoreState
    {
        // Actual State Data
        public Dictionary<string, AcePlayer> Players { get; private set; } // keyed by PlayerId (not peer)
        public Dictionary<string, AcePlane> PlanesById { get; private set; }
        public AceBoard Board { get; private set; }
        public AcePlayer CurrentPlayer { get; private set;}

        // Ephemeral/calculated stuff

        public AceCoreState()
        {
             Players = new Dictionary<string, AcePlayer>();
        }

        public class SerialArgs
        {
            public long cmdSeqNum;
            public SerialArgs(long sn ) {cmdSeqNum=sn; }
        };

        public override string ApianSerialized(object args=null)
        {

            // State data
            string[] playersData = Players.Values.OrderBy(p => p.PlayerId)
                .Select(p => p.ApianSerialized()).ToArray();

            return  JsonConvert.SerializeObject(new object[]{
                ApianSerializedBaseData(), // serialize all of the AppCoreBase data
                playersData
            });
        }

        public static AceCoreState FromApianSerialized( long seqNum,  string stateHash,  string serializedData)
        {
            AceCoreState newState = new AceCoreState();

            JArray sData = JArray.Parse(serializedData);

            newState.ApplyDeserializedBaseData((string)sData[0]); // Populate the base ApianCoreState  data

            Dictionary<string, AcePlayer> newPlayers = (sData[1] as JArray)
                .Select( s => AcePlayer.FromApianJson((string)s))
                .ToDictionary(p => p.PlayerId);

            newState.Players = newPlayers;

            return newState;
        }

    }

}
using System;
using System.Linq;
using System.Collections.Generic;
using Apian;
using UniLog;
using static UniLog.UniLogger; // for SID()

namespace AceGameCode
{
    public class AceCoreState : ApianCoreState
    {
        // Actual State Data
        public Dictionary<string, AcePlayer> PlayersById { get; private set; }
        public Dictionary<string, AcePlane> PlanesById { get; private set; }
        public AceBoard Board { get; private set; }
        public AcePlayer CurrentPlayer { get; private set;}

        // Ephemeral/calculated stuff
        public UniLogger Logger;

        public AceCoreState()
        {
            Logger = UniLogger.GetLogger("GameState");
            PlayersById = new Dictionary<string, AcePlayer>();
        }

        public override string ApianSerialized(object args=null)
        {
            return null;
        }

        public static AceCoreState FromApianSerialized( long seqNum,  string stateHash,  string serializedData)
        {
            AceCoreState newState = new AceCoreState();
            return newState;
        }

    }

}
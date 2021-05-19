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
        public Dictionary<string, AcePlayer> PlayersById { get; private set; } = null;

        // Ephemeral/calculated stuff
        public UniLogger Logger;

        public AceCoreState()
        {
            Logger = UniLogger.GetLogger("GameState");
            PlayersById = new Dictionary<string, AcePlayer>();
        }

    }

}
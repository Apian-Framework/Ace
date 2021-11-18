using System.Collections.Generic;
using Newtonsoft.Json;
using GameNet;
using Apian;
using UniLog;
using UnityEngine;

namespace AceGameCode
{
    public class AceApianSinglePeer : AceApian
    {
        public AceApianSinglePeer(IAceGameNet _gn,  AceAppCore _client) : base(_gn, _client)
        {
            ApianClock = new CoopApianClock(this); // FIXME: Ace needs a virtual timer-based clock
            GroupMgr = new SinglePeerGroupManager(this);
        }

        public override (bool, string) CheckQuorum()
        {
            return (true, "");
        }

    }
}
using System.Collections.Generic;
using Newtonsoft.Json;
using GameNet;
using Apian;
using UniLog;
using UnityEngine;

namespace AceGameCode
{
    public class AceApianCreatorSez : AceApian
    {
        public AceApianCreatorSez(IAceGameNet _gn,  IAceAppCore _client) : base(_gn, _client)
        {
            // TODO: LeaderClock needs a way to set the leader. Currently uses group creator.
            ApianClock = new LeaderApianClock(this); // For Ace this should be a virtual-timer-based clock rather than realtime
            GroupMgr = new CreatorSezGroupManager(this);
        }

        public override (bool, string) CheckQuorum()
        {
            AceGameInfo agi = GroupInfo as AceGameInfo;

            if ( GroupMgr.GetMember(agi.GroupCreatorId) == null)
                return (false, $"Creator Peer {agi.GroupCreatorId} not present");

            if ( GroupMgr.ActiveMemberCount < (agi.MaxPlayers + agi.MinValidators) )
                return (false, $"Not enough peers: {GroupMgr.ActiveMemberCount}. Need {(agi.MaxPlayers + agi.MinValidators)}");

            return (true, "");
        }
    }
}
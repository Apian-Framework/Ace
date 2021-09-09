using System;
using System.Text;
using System.Security.Cryptography; // for MD5 hash
using System.Collections.Generic;
using Apian;
using Newtonsoft.Json;
using UnityEngine;
using UniLog;
using static UniLog.UniLogger; // for SID()

namespace AceGameCode
{
    public class AceApianPeer : ApianGroupMember
    {
        public AceApianPeer(string _p2pId, string _appHelloData) : base(_p2pId, _appHelloData) { }
    }

    public static class AceApianFactory
    {
        public static readonly List<string> ApianGroupTypes = new List<string>()
        {
            SinglePeerGroupManager.kGroupType,
            CreatorSezGroupManager.kGroupType,
            // LeaderSezGroupManager.kGroupType
        };

        public static AceApian Create(string apianGroupType, IAceGameNet aceGameNet, AceAppCore appCore)
        {
            AceApian result;
            switch (apianGroupType)
            {
            case SinglePeerGroupManager.kGroupType:
                result = new AceApianSinglePeer(aceGameNet, appCore);
                break;
            case CreatorSezGroupManager.kGroupType:
                result =  new AceApianCreatorSez(aceGameNet, appCore);
                break;
            // case LeaderSezGroupManager.kGroupType:
            //     result =  new AceApianLeaderSez(aceGameNet, appCore);
            //     break;
            default:
                UniLogger.GetLogger("Apian").Warn($"AceApianFactory.Create() Unknown GroupType: {apianGroupType}");
                result = null;
                break;
            }
            return result;
        }
    }

    public abstract class AceApian : ApianBase
    {

        public Dictionary<string, AceApianPeer> apianPeers;
        public IAceGameNet AceGameNet {get; private set;}
        protected AceAppCore appCore;

        public long SystemTime { get => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;}  // system clock

        protected AceApian(IAceGameNet _gn, IAceAppCore _client) : base(_gn, _client as ApianAppCore)
        {
            AceGameNet = _gn;
            appCore = _client as AceAppCore;

            // ApianClock and GroupMgr are created in the group-manager-specific subclass
            // ie. AceApianLeaderSez

            apianPeers = new Dictionary<string, AceApianPeer>();
        }


        public override void Update()
        {
            GroupMgr?.Update();
            ApianClock?.Update();
            // ((AceAppCore)AppCore)?.Loop();   <- ace AppCore has no loop()
        }

        protected void AddApianPeer(string p2pId, string peerHelloData)
        {
            AceApianPeer p = new AceApianPeer(p2pId, peerHelloData);
            apianPeers[p2pId] = p;
        }

        public override void OnPeerMissing(string channelId, string p2pId)
        {
            Logger.Warn($"Peer: {SID(p2pId)} is missing!");
            appCore.OnPeerMissing(channelId, p2pId);
        }

        public override void OnPeerReturned(string channelId, string p2pId)
        {
            Logger.Warn($"Peer: {SID(p2pId)} has returned!");
            appCore.OnPeerReturned(channelId, p2pId);
        }


        // Called FROM GroupManager
        public override string ValidateJoinRequest( GroupJoinRequestMsg requestMsg)
        {
            AcePlayer pl = AcePlayer.FromApianJson(requestMsg.ApianClientPeerJson);
            AceGameInfo agi = GroupInfo  as AceGameInfo;
            const int kMaxPlayers = 2; // kinda implicit in the game

            // Is there room for a player?
            if ( pl.Role == PlayerRole.kPlayer && (AppCore as AceAppCore).CoreState.Players.Count >= kMaxPlayers)
                return "Max Players exceeded";

            // How about for a Validator?
            if ((pl.Role == PlayerRole.kPlayer || pl.Role == PlayerRole.kPreferPlayer)
                && GroupMgr.MemberCount >= kMaxPlayers + agi.MaxValidators)
                return "Max Validators exceeded.";

            return null; // let 'em in

        }

        public override void OnGroupMemberJoined(ApianGroupMember member) // ATM Ace doesn't care
        {
            base.OnGroupMemberJoined(member);

            // Ace (appCore) only cares about local peer's group membership status
            if (member.PeerId == GameNet.LocalP2pId())
            {
                appCore.OnGroupJoined(GroupMgr.GroupId);  // TODO: wait - appCore has OnGroupJoined? SHouldn't know about groups.
            }
        }

        public override void OnGroupMemberStatusChange(ApianGroupMember member, ApianGroupMember.Status prevStatus)
        {
            base.OnGroupMemberStatusChange(member, prevStatus);
            // Note that the member status has already been changed when this is called

            // Ace-specific handling.
            // Joining->Active : PlayerJoined
            // Active->Removed : PlayerLeft

            //  When we see these transitions send an OBSERVATION.
            // The ApianGroup will can then convert it to a Command and distribute that.

             switch(prevStatus)
            {
            case ApianGroupMember.Status.Joining:
                if (member.CurStatus == ApianGroupMember.Status.Active)
                {
                    // In a leader-based ApianGroup the first peer will probably go stright from Joining to Active
                    SendNewPlayerObs(ApianClock.CurrentTime, AcePlayer.FromApianJson(member.AppDataJson));
                }
                break;
            case ApianGroupMember.Status.Syncing:
                if (member.CurStatus == ApianGroupMember.Status.Active)
                {
                    // Most common situation
                    SendNewPlayerObs(ApianClock.CurrentTime, AcePlayer.FromApianJson(member.AppDataJson));
                }
                break;
            case ApianGroupMember.Status.Active:
                if (member.CurStatus == ApianGroupMember.Status.Removed)
                {
                    // FIXME: This switch needs to all be turned inside-out.
                    // If ANY state transitions to "removed" then PlayerLeft needs to get sent
                    SendPlayerLeftObs(ApianClock.CurrentTime, member.PeerId);
                }
                break;
            }
        }


        // State checkpoints
        private static string _GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public override void SendCheckpointState(long timeStamp, long seqNum, string serializedState) // called by client app
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = _GetMd5Hash(md5Hash, serializedState);
                Logger.Verbose($"SendStateCheckpoint(): SeqNum: {seqNum}, Hash: {hash}");
                GroupMgr.OnLocalStateCheckpoint(seqNum, timeStamp, hash, serializedState);

                GroupCheckpointReportMsg rpt = new GroupCheckpointReportMsg(GroupMgr.GroupId, seqNum, timeStamp, hash);
                AceGameNet.SendApianMessage(GroupMgr.GroupId, rpt);
            }
        }

        public void SendNewPlayerObs(long timeStamp, AcePlayer newPlayer)
        {
            Logger.Debug($"SendNewPlayerObs()");
            NewPlayerMsg msg = new NewPlayerMsg(timeStamp, newPlayer);
            SendObservation( msg);
        }

        public void SendPlayerLeftObs( long timeStamp, string peerId)
        {
            Logger.Debug($"SendPlayerLeftObs()");
            PlayerLeftMsg msg = new PlayerLeftMsg(timeStamp, peerId);
            SendObservation( msg);
        }

        public void SendNewPlayerRequest(AcePlayer newPlayer)
        {
            Logger.Debug($"SendNewPlayerRequest()");
            NewPlayerMsg msg = new NewPlayerMsg(ApianClock.CurrentTime, newPlayer);
            SendRequest(msg);
        }
    }
}
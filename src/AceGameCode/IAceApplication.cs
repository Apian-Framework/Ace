using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apian;
using GameNet;

namespace AceGameCode
{
    public class AceGameInfo : ApianGroupInfo
    {
        public string GameName { get => GroupName; }
        public int MaxPlayers { get => int.Parse(GroupParams["MaxPlayers"]); set => GroupParams["MaxPlayers"] = $"{value}";}
        public int MinValidators { get => int.Parse(GroupParams["MinValidators"]); set => GroupParams["MinValidators"] = $"{value}";}
        public int ValidatorWaitMs { get => int.Parse(GroupParams["ValidatorWaitMs"]); set => GroupParams["ValidatorWaitMs"] = $"{value}";}
        public AceGameInfo(ApianGroupInfo agi) : base(agi) {}
    }

    public class AceGameStatus : ApianGroupStatus
    {
        public int PlayerCount { get => int.Parse(OtherStatus["PlayerCount"]); set => OtherStatus["PlayerCount"] = $"{value}"; }
        public int ValidatorCount { get => int.Parse(OtherStatus["ValidatorCount"]); set => OtherStatus["ValidatorCount"] = $"{value}";}
        public AceGameStatus(ApianGroupStatus ags) : base(ags) {}
    }

    public class AceGameAnnounceData
    {
        public AceGameInfo GameInfo { get; }
        public AceGameStatus GameStatus { get; }
        public AceGameAnnounceData(GroupAnnounceResult gar)
        {
            GameInfo = new AceGameInfo(gar.GroupInfo);
            GameStatus = new AceGameStatus(gar.GroupStatus);
        }
    }


    public class PeerJoinedEventArgs : EventArgs {
        public string channelId;
        public AceNetworkPeer peer;
        public PeerJoinedEventArgs(string g, AceNetworkPeer p) {channelId=g; peer=p;}
    }
    public class PeerLeftEventArgs : EventArgs {
        public string channelId;
        public string p2pId;
        public PeerLeftEventArgs(string g, string p) {channelId=g; p2pId=p;}
    }

    public class GameAnnounceEventArgs : EventArgs {
        public AceGameInfo gameInfo;
        public GameAnnounceEventArgs( AceGameInfo gi) { gameInfo = gi; }
    }

    public class GameSelectedEventArgs : EventArgs {
        public enum ReturnCode {kCreate, kJoin, kCancel, kMaxPlayers};
        public ReturnCode result;
        public AceGameInfo gameInfo;
        public GameSelectedEventArgs( AceGameInfo gi, ReturnCode r) { gameInfo = gi; result = r; }
    }

    public class LocalPeerJoinedGameData {
        public string groupId;
        public bool success;
        public string failureReason;
        public LocalPeerJoinedGameData(string gId,  bool result,  string fr)
        {
            groupId = gId;
            success = result;
            failureReason = fr;
        }
    }

    public interface IAceApplication : IApianApplication
    {
        // Events
        event EventHandler<PeerJoinedEventArgs> PeerJoinedEvt;
        event EventHandler<PeerLeftEventArgs> PeerLeftEvt;
        event EventHandler<GameAnnounceEventArgs> GameAnnounceEvt;

        IAceGameNet aceGameNet {get;}

        void ConnectToNetwork(string netConnectionStr);
        Task<PeerJoinedNetworkData> JoinGameNetworkAsync(string networkName);
        Task<Dictionary<string, AceGameAnnounceData>> GetExistingGamesAsync(int waitMs);
        Task<GameSelectedEventArgs> SelectGameAsync(IDictionary<string, AceGameAnnounceData> existingGames);
        void ExitApplication(); // relatively controlled exit via modeMgr



    }

}

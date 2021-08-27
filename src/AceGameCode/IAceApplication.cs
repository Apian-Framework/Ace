using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apian;
using GameNet;

namespace AceGameCode
{

    public class AceGameInfo
    {
        public ApianGroupInfo GroupInfo;
        public string GameName { get => GroupInfo.GroupName; }
        public AceGameInfo(ApianGroupInfo agi) { GroupInfo = agi; }
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
    public class GameSelectedEventArgs : EventArgs {
        public enum ReturnCode {kCreate, kJoin, kCancel};
        public ReturnCode result;
        public AceGameInfo gameInfo;
        public GameSelectedEventArgs( AceGameInfo gi, ReturnCode r) { gameInfo = gi; result = r; }
    }

    public class GameAnnounceEventArgs : EventArgs {
        public AceGameInfo gameInfo;
        public GameAnnounceEventArgs( AceGameInfo gi) { gameInfo = gi; }
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
        Task<Dictionary<string, AceGameInfo>> GetExistingGamesAsync(int waitMs);
        Task<GameSelectedEventArgs> SelectGameAsync(IDictionary<string, AceGameInfo> existingGames);
        void ExitApplication(); // relatively controlled exit via modeMgr



    }

}

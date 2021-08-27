using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Apian;
using ModalApplication;
using GameNet;
using UniLog;
using static UniLog.UniLogger; // for SID()

namespace AceGameCode
{

    public class AceApplication: IFsmApp, IGameNetClient, IAceApplication
    {
        public IAceGameNet aceGameNet {get; private set;}
        public IAceFrontend frontend {get; private set;}
        public AceAppCore mainAppCore {get; private set;}
        public UniLogger Logger { get; private set; }
        public AceNetworkPeer LocalPeer { get; private set; }

        public FsmModeManager appModeMgr {get; private set;}

        public event EventHandler<PeerJoinedEventArgs> PeerJoinedEvt;
        public event EventHandler<PeerLeftEventArgs> PeerLeftEvt;
        public event EventHandler<GameAnnounceEventArgs> GameAnnounceEvt;

        public AceApplication(AceGameNet agn, IAceFrontend fe)
        {
            Logger = UniLogger.GetLogger("AceApplication");
            Logger.Verbose($"Ctor: AceApplication({agn}, {fe})");
            aceGameNet = agn;
            aceGameNet.AddClient(this);
            frontend = fe;
            appModeMgr = new FsmModeManager(new AceModeFactory(), this);

            frontend.SetAceApplication(this);
        }

        // IModalApp
       public void Start(int initialMode)
        {
            Logger.Info($"Start({initialMode})");
            appModeMgr.Start(initialMode);
        }

        public void End() {}

        public bool IsRunning {get => appModeMgr.CurrentModeId() != -1;}

        // IAceApplication

        public void ConnectToNetwork(string netConnectionStr)
        {
           // Connect is (for now) synchronous
            aceGameNet.Connect(netConnectionStr);
        }
        public async Task<PeerJoinedNetworkData> JoinGameNetworkAsync(string networkName)
        {
            _CreateLocalPeer(); // takes data from settings and p2p instance
            return await aceGameNet.JoinGameNetworkAsync(networkName, LocalPeer);
        }

        public async Task<Dictionary<string, AceGameInfo>> GetExistingGamesAsync(int waitMs)
        {
            Dictionary<string, ApianGroupInfo> groupsDict = await aceGameNet.RequestGroupsAsync(waitMs);
            Dictionary<string, AceGameInfo> gameDict = groupsDict.Values.Select((grp) => new AceGameInfo(grp)).ToDictionary(gm => gm.GameName, gm => gm);
            Logger.Info($"GetExistingGamesAsync() Got result:\n  {string.Join(Environment.NewLine, gameDict)}");
            return gameDict;
        }

        public async Task<GameSelectedEventArgs> SelectGameAsync(IDictionary<string, AceGameInfo> existingGames)
        {
            GameSelectedEventArgs selection = await frontend.SelectGameAsync(existingGames);
            Logger.Info($"SelectGameAsync() Got result:  GameName: {selection.gameInfo.GameName} ResultCode: {selection.result}");
            return selection;
        }

        protected AcePlayer MakeAcePlayer() => new AcePlayer(LocalPeer.PeerId, LocalPeer.Name);
        // FIXME: I think maybe it should go in BeamGameNet?

        public void CreateAndJoinGame(AceGameInfo gameInfo, AceAppCore appCore)
        {
            aceGameNet.CreateAndJoinGame(gameInfo, appCore.AceApian, MakeAcePlayer().ApianSerialized() );
        }
       public void JoinExistingGame(AceGameInfo gameInfo, AceAppCore appCore)
        {
            aceGameNet.JoinExistingGame(gameInfo, appCore.AceApian, MakeAcePlayer().ApianSerialized() );
        }

        public void ExitApplication()
        {
            appModeMgr.Stop();
        }

        private void _CreateLocalPeer()
        {
            AceUserSettings settings = frontend.GetUserSettings();
            LocalPeer = new AceNetworkPeer(aceGameNet.LocalP2pId(), settings.screenName);
        }

        // IApianApplication
        public void AddAppCore(IApianAppCore ac)
        {
            // Beam only supports 1 game instance
            mainAppCore = ac as AceAppCore;
            //frontend.SetAppCore(ac as IAceAppCore); /// TODO: this is just a hack. BUT WE STILL NEED TO DO IT for now
        }
        public void OnGroupAnnounce(ApianGroupInfo groupInfo)
        {
            Logger.Info($"OnGroupAnnounce({groupInfo?.GroupName})");
            AceGameInfo bgi = new AceGameInfo(groupInfo);
            GameAnnounceEvt?.Invoke(this, new GameAnnounceEventArgs(bgi));
        }
        public void OnGroupMemberStatus(string groupId, string peerId, ApianGroupMember.Status newStatus, ApianGroupMember.Status prevStatus)
        {
            Logger.Info($"OnGroupMemberStatus() Grp: {groupId}, Peer: {UniLogger.SID(peerId)}, Status: {newStatus}, Prev: {prevStatus}");
        }

          // IGameNetClient
        public void OnPeerJoinedNetwork(PeerJoinedNetworkData peerData)
        {
            AceNetworkPeer peer = JsonConvert.DeserializeObject<AceNetworkPeer>(peerData.HelloData);
            Logger.Info($"OnPeerJoinedNetwork() {((peerData.PeerId == LocalPeer.PeerId)?"Local":"Remote")} name: {peer.Name}");
            PeerJoinedEvt?.Invoke(this, new PeerJoinedEventArgs(peerData.NetId, peer));
        }

        public void OnPeerLeftNetwork(string p2pId, string netId)
        {
            Logger.Info($"OnPeerLeftGame({SID(p2pId)})");
            PeerLeftEvt?.Invoke(this, new PeerLeftEventArgs(netId, p2pId)); // Event instance might be gone
        }

        public void OnPeerMissing(string p2pId, string netId) { }
        public void OnPeerReturned(string p2pId, string netId){ }
        public void OnPeerSync(string channel, string p2pId, long clockOffsetMs, long netLagMs) {} // stubbed


    }

}
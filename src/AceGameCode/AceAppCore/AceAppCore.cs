using System;
using System.Linq;
using System.Collections.Generic;
using Apian;
using UniLog;
using ModalApplication; // The Ace AppCore *IS* a ModalApplication
using static UniLog.UniLogger; // for SID()

namespace AceGameCode
{
    //
    // Event args
    //
    public class StringEventArgs : EventArgs
    {
        public string str;
        public StringEventArgs(string s) {str = s; }
    }
    public class PlayerJoinedEventArgs : EventArgs {
        public string groupChannel;
        public AcePlayer player;
        public PlayerJoinedEventArgs(string g, AcePlayer p) {groupChannel=g; player=p;}
    }
    public class PlayerLeftEventArgs : EventArgs {
        public string groupChannel;
        public string playerId;
        public string peerId;
        public PlayerLeftEventArgs(string g, string plId, string p2pId) {groupChannel=g; playerId=plId; peerId=p2pId;}
    }

    public interface IAceAppCore
    {
        event EventHandler<StringEventArgs> GroupJoinedEvt;
        event EventHandler<PlayerJoinedEventArgs> PlayerJoinedEvt;
        event EventHandler<PlayerLeftEventArgs> PlayerLeftEvt;
        event EventHandler<PlayerLeftEventArgs> PlayerMissingEvt; // not Gone... yet
        event EventHandler<PlayerLeftEventArgs> PlayerReturnedEvt;

        //AceCoreState CoreState { get;}
    }

    public class AceAppCore : ApianAppCore, IAceAppCore, IFsmApp
    {
        public event EventHandler<StringEventArgs> GroupJoinedEvt;
        public event EventHandler<PlayerJoinedEventArgs> PlayerJoinedEvt;
        public event EventHandler<PlayerLeftEventArgs> PlayerLeftEvt;
        public event EventHandler<PlayerLeftEventArgs> PlayerMissingEvt; // not Gone... yet
        public event EventHandler<PlayerLeftEventArgs> PlayerReturnedEvt;
        public AceApian AceApian {get; private set;}

        public FsmModeManager CoreModeMgr {get; private set;}

        //public AcePlayer LocalPlayer { get; private set; }

        public UniLogger Logger {get; private set;}
        public AceCoreState CoreState {get; private set;}

        public long FrameApianTime { get => AceApian.ApianClock.CurrentTime; }

        public AceAppCore()
        {
            Logger = UniLogger.GetLogger("AppCore"); // TODO: should thins be in a (currently nonexistent) base ctor?

            CoreModeMgr = new FsmModeManager(new AceCoreModeFactory(), this);

            CoreState = new AceCoreState();
            OnNewCoreState();

            ClientMsgCommandHandlers = new  Dictionary<string, Action<ApianCoreMessage, long>>()
            {
                [AceMessage.kNewPlayer] = (msg, seqNum) => OnNewPlayerCmd(msg as NewPlayerMsg, seqNum),
                // [BeamMessage.kPlayerLeft] = (msg, seqNum) => OnPlayerLeftCmd(msg as PlayerLeftMsg, seqNum),
                // [BeamMessage.kBikeCreateData] = (msg, seqNum) => this.OnCreateBikeCmd(msg as BikeCreateMsg, seqNum),
                // [BeamMessage.kRemoveBikeMsg] = (msg, seqNum) => this.OnRemoveBikeCmd(msg as RemoveBikeMsg, seqNum),
                // [BeamMessage.kBikeTurnMsg] = (msg, seqNum) => this.OnBikeTurnCmd(msg as BikeTurnMsg, seqNum),
                // [BeamMessage.kBikeCommandMsg] =(msg, seqNum) => this.OnBikeCommandCmd(msg as BikeCommandMsg, seqNum),
                // [BeamMessage.kPlaceClaimMsg] = (msg, seqNum) => this.OnPlaceClaimCmd(msg as PlaceClaimMsg, seqNum),
                // [BeamMessage.kPlaceHitMsg] = (msg, seqNum) => this.OnPlaceHitCmd(msg as PlaceHitMsg, seqNum),
                // [BeamMessage.kPlaceRemovedMsg] = (msg, seqNum) => this.OnPlaceRemovedCmd(msg as PlaceRemovedMsg, seqNum),
                [ApianMessage.CheckpointMsg] = (msg, seqNum) => this.OnCheckpointCommand(msg as ApianCheckpointMsg, seqNum),
            };
        }

        //
        // IFsmApp
        //

        public bool IsRunning {get => CoreModeMgr.CurrentModeId() != -1;}

        public void Start(int initialMode)
        {
            Logger.Info($"Fsm Start({initialMode})");
            CoreModeMgr.Start(initialMode);
        }

        public void End()
        {
            Logger.Info($"Fsm End()");
         }


        //
        // ClientMsg command handlers
        //
        public void OnNewPlayerCmd(NewPlayerMsg msg, long seqNum)
        {
            AcePlayer newPlayer = msg.newPlayer;
            Logger.Info($"OnNewPlayerCmd(#{seqNum}) ID: {SID(newPlayer.PlayerId)} Name: {newPlayer.Name} Peer: {SID(newPlayer.PeerId)}");
            _AddPlayer(newPlayer);
        }


        // IApianAppCore
        public override void SetApianReference(ApianBase ap)
        {
            base.SetApianReference(ap);
            AceApian = ap as AceApian;
        }

        protected override void OnNewCoreState(ApianCoreState _ = null)
        {
            base.OnNewCoreState(CoreState);

            // Do Ace-dependent stuff here, particularly any event subscribing

        }

        public override ApianCoreMessage DeserializeCoreMessage(ApianWrappedCoreMessage aMsg)
        {
            return AceCoreMessageDeserializer.FromJSON(aMsg.CoreMsgType, aMsg.SerializedCoreMsg);
        }

        public override bool CommandIsValid(ApianCoreMessage cmdMsg)
        {
            throw new NotImplementedException();
        }
        public override void OnApianCommand(long cmdSeqNum, ApianCoreMessage coreMsg)
        {
            Logger.Debug($"OnApianCommand() Seq#: {cmdSeqNum} Cmd: {coreMsg.MsgType}");
            CoreState.UpdateCommandSequenceNumber(cmdSeqNum);
            ClientMsgCommandHandlers[coreMsg.MsgType](coreMsg, cmdSeqNum);

        }

        // what effect does the previous msg have on the testMsg?
        public override (ApianConflictResult result, string reason) ValidateCoreMessages(ApianCoreMessage prevMsg, ApianCoreMessage testMsg)
        {
            throw new NotImplementedException();
            //return BeamMessageValidity.ValidateObservations( prevMsg as BeamMessage, testMsg as BeamMessage);
        }

        public override void OnCheckpointCommand(ApianCheckpointMsg msg, long seqNum)
        {
            // TODO: if every AppCore uses this then it should be hoisted to the base class
            Logger.Info($"OnCheckpointCommand() seqNum: {seqNum}, timestamp: {msg.TimeStamp}, Now: {FrameApianTime}");
            string stateJson = CoreState.ApianSerialized(new AceCoreState.SerialArgs(seqNum));
            Logger.Debug($"**** Checkpoint:\n{stateJson}\n************\n");
            AceApian.SendCheckpointState(FrameApianTime, seqNum, stateJson);

        }
        public override void ApplyCheckpointStateData(long seqNum,  long timeStamp,  string stateHash,  string serializedData)
        {
            Logger.Debug($"ApplyStateData() Seq#: seqNum ApianTime: {timeStamp}");
            CoreState = AceCoreState.FromApianSerialized(seqNum,  stateHash,  serializedData);
            OnNewCoreState(); // send NewCoreStateEvt

            // New player events
            foreach (AcePlayer p in CoreState.Players.Values)
            {
                PlayerJoinedEvt.Invoke(this, new PlayerJoinedEventArgs(ApianGroupId, p));
            }

        }

        // AceAppCore stuff
        public void OnGroupJoined(string groupId)
        {
            Logger.Info($"OnGroupJoined({groupId}) - local peer joined");
            GroupJoinedEvt?.Invoke(this, new StringEventArgs(groupId));
        }


        public void OnPeerMissing(string groupId, string p2pId)
        {
            Logger.Info($"Peer: {SID(p2pId)} is missing!");
            foreach( AcePlayer pl in CoreState.Players.Values.Where(p => p.PeerId == p2pId))
                PlayerMissingEvt?.Invoke(this, new PlayerLeftEventArgs(groupId, pl.PlayerId, pl.PeerId));
        }

        public void OnPeerReturned(string groupId, string p2pId)
        {
            Logger.Info($"Peer: {SID(p2pId)} has returned!");
            foreach( AcePlayer pl in CoreState.Players.Values.Where(p => p.PeerId == p2pId))
                PlayerReturnedEvt?.Invoke(this, new PlayerLeftEventArgs(groupId, pl.PlayerId, pl.PeerId));
        }

        //

        // Peer-related
        private bool _AddPlayer(AcePlayer p)
        {
            Logger.Debug($"_AddPlayer().  ID: {SID(p.PlayerId)} Name: {p.Name} Peer: {SID(p.PeerId)}");
            if  ( CoreState.Players.ContainsKey(p.PlayerId))
            {
                Logger.Warn($"_AddPlayer(). Player already exists!!!!");
                return false;
            }

            CoreState.Players[p.PlayerId] = p;
            PlayerJoinedEvt.Invoke(this, new PlayerJoinedEventArgs(ApianGroupId, p));
            return true;
        }

    }
}
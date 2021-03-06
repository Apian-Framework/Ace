using System.Collections.Generic;
using System;
using System.Linq;
using ModalApplication;
using Apian;
using UnityEngine;
using static UniLog.UniLogger; // for SID()

namespace AceGameCode
{
    public class ModePlay : AceGameMode
    {
        // Mode begins with no AppCore, no Apian, not connection, nothing...
        // It has: net connection string and the name of the desired ApianNetwork to join.

        private AceUserSettings settings ;
        private Dictionary<string, AceGameInfo> announcedGames;

        protected const float kListenForGamesSecs = 2.0f; // TODO: belongs here?

		public async override void Start(object param = null)
        {
            base.Start();
            announcedGames = new Dictionary<string, AceGameInfo>();

            settings = appl.frontend.GetUserSettings();
            appl.AddAppCore(null);

            try {
                appl.ConnectToNetwork(settings.p2pConnectionString); // should be async? GameNet.Connect() currently is not
                GameNet.PeerJoinedNetworkData netJoinData = await appl.JoinGameNetworkAsync(settings.apianNetworkName);

                Dictionary<string, AceGameAnnounceData> gamesAvail = await appl.GetExistingGamesAsync((int)(kListenForGamesSecs*1000));
                GameSelectedEventArgs selection = await appl.SelectGameAsync(gamesAvail);

                if (selection.result == GameSelectedEventArgs.ReturnCode.kCancel)
                    ExitAbruptly($"No Game Selected.");

                AceGameInfo gameInfo = selection.gameInfo;
                AceAppCore appCore = _SetupCorePair(gameInfo);

                bool targetGameExisted = (gameInfo.GameName != null) && gamesAvail.ContainsKey(gameInfo.GameName);

                LocalPeerJoinedGameData gameJoinedResult = null;

                bool isValidator = settings.tempSettings.TryGetValue("validator", out var value) ? Convert.ToBoolean(value) : false;

                switch (selection.result)
                {
                case  GameSelectedEventArgs.ReturnCode.kCreate:
                    // Create and join
                    if (targetGameExisted)
                        ExitAbruptly($"Cannot create.  Beam Game \"{gameInfo.GameName}\" already exists");
                    else
                        gameJoinedResult = await appl.CreateAndJoinGameAsync(gameInfo, appCore);
                    break;

                case GameSelectedEventArgs.ReturnCode.kJoin:
                    // Join existing
                    if (!targetGameExisted)
                    {
                         ExitAbruptly($"Cannot Join.  Beam Game \"{gameInfo.GameName}\" not found");
                         return;
                    }
                    else
                        gameJoinedResult = await appl.JoinExistingGameAsync(gameInfo, appCore);
                    break;

                case GameSelectedEventArgs.ReturnCode.kMaxPlayers:
                    gameJoinedResult = new LocalPeerJoinedGameData(gameInfo.GroupId, false,
                            $"Cannot Join as player. Beam Game \"{gameInfo.GameName}\" already has {gameInfo.MaxPlayers} players.");
                    break;

                case GameSelectedEventArgs.ReturnCode.kCancel:
                    gameJoinedResult = new LocalPeerJoinedGameData(gameInfo.GroupId, false, "Join Cancelled");
                    break;
                }

                if (!gameJoinedResult.success)
                {
                    ExitAbruptly( gameJoinedResult.failureReason);
                    return;
                }

                if (isValidator)
                      logger.Info($"Validator setting is set. Will not create a player.");
                else
                {
                    logger.Info($"Requesting new player.");
                    PlayerJoinedEventArgs joinData = await appl.CreateNewPlayerAsync( appCore, gameJoinedResult.groupId, appl.MakeAiPlayer() );
                    if (joinData == null) {
                        ExitAbruptly("Failed to Create New Player");
                        return;
                    }
                }


            } catch (Exception ex) {
                ExitAbruptly( $"{ex.Message}");

                return;
            }
        }


        private AceAppCore _SetupCorePair(AceGameInfo gameInfo)
        {
            if (gameInfo == null)
                ExitAbruptly($"_SetupCorePair(): null gameInfo");

            AceAppCore appCore = CreateCorePair(gameInfo);
            appl.AddAppCore(appCore);
            appCore.PlayerJoinedEvt += _OnPlayerJoinedEvt;
            appCore.Start(AceCoreModeFactory.kStart );
            return appCore;
        }

        // AppCore event handlers
        private void _OnPlayerJoinedEvt(object sender, PlayerJoinedEventArgs ga)
        {
            bool isLocal = ga.player.PeerId == appl.LocalPeer.PeerId;
            logger.Info($"{(ModeName())} - OnPlayerJoinedEvt() - {(isLocal?"Local":"Remote")} Member Joined: {ga.player.Name}, ID: {SID(ga.player.PeerId)}");
            if (ga.player.PeerId == appl.LocalPeer.PeerId)
            {

            }
        }

    }
}



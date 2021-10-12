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

        // - Connect to Gamenet
        // - Join the ApianNet. Wait for
        //     -> OnPeerJoinedNetwork()
        // - Get a list of available games
        // - Ask FE for a game to create/join. Wait for:
        //     -> OnGameSelected()
        // - Create/Join game. Wait for:
        //     -> OnPlayerJoinedEvt()
        // - Start playing  (could wait for others to join...)

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

                Dictionary<string, AceGameInfo> gamesAvail = await appl.GetExistingGamesAsync((int)(kListenForGamesSecs*1000));
                GameSelectedEventArgs selection = await appl.SelectGameAsync(gamesAvail);

                if (selection.result == GameSelectedEventArgs.ReturnCode.kCancel)
                    ExitAbruptly($"No Game Selected.");

                AceGameInfo gameInfo = selection.gameInfo;
                AceAppCore appCore = _SetupCorePair(gameInfo);

                bool targetGameExisted = (gameInfo.GameName != null) && gamesAvail.ContainsKey(gameInfo.GameName);

                LocalPeerJoinedGameData gameJoinedResult = null;

                if (selection.result == GameSelectedEventArgs.ReturnCode.kCreate)
                {
                    // Create and join
                    if (targetGameExisted)
                        ExitAbruptly($"Cannot create.  Beam Game \"{gameInfo.GameName}\" already exists");
                    else
                        gameJoinedResult = await appl.CreateAndJoinGameAsync(gameInfo, appCore);

                } else {
                    // Join existing
                    if (!targetGameExisted)
                         ExitAbruptly($"Cannot Join.  Beam Game \"{gameInfo.GameName}\" not found");
                    else
                        gameJoinedResult = await appl.JoinExistingGameAsync(gameInfo, appCore);
                }

                if (!gameJoinedResult.success)
                    ExitAbruptly( gameJoinedResult.failureReason);

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

/*
        public async void OnGameSelected( GameSelectedEventArgs args)
        {
            AceGameInfo gameInfo = args.gameInfo;
            GameSelectedEventArgs.ReturnCode result = args.result;
            string gameName = gameInfo?.GroupName;

            logger.Info($"{(ModeName())} - OnGameSelected(): {gameName}, result: {result}");

            bool targetGameExisted = (gameName != null) && announcedGames.ContainsKey(gameName);

            if (result == GameSelectedEventArgs.ReturnCode.kCancel)
            {
                ExitAbruptly( $"OnGameSelected(): No Game Selected.");
            }
            else
            {
                AceAppCore appCore = null;
                if (gameInfo != null)
                {
                    appCore = CreateCorePair(gameInfo);
                    appl.AddAppCore(appCore);
                    appCore.PlayerJoinedEvt += _OnPlayerJoinedEvt;
                    appCore.Start(AceCoreModeFactory.kStart );
                }

                LocalPeerJoinedGameData joinData = null;

                switch (result)
                {
                case GameSelectedEventArgs.ReturnCode.kCreate:
                    if (targetGameExisted)
                        ExitAbruptly( $"OnGameSelected(): Cannot create.  Ace Game \"{gameName}\" already exists");
                    else {
                        joinData = await appl.CreateAndJoinGameAsync(gameInfo, appCore);
                    }
                    break;

                case GameSelectedEventArgs.ReturnCode.kJoin:
                    if (targetGameExisted)
                    {
                        joinData = await appl.JoinExistingGameAsync(gameInfo, appCore);
                    }
                    else
                        ExitAbruptly( $"OnGameSelected(): Apian Game \"{gameName}\" Not Found");
                    break;
                }

                if (joinData?.success == false)
                    ExitAbruptly( $"ModePlay: Failed to join Apian group: \"{joinData?.failureReason}\"");
            }
        }
*/

        // private void _OnLocalGameJoinedEvt(object sender, LocalPeerJoinedGameEventArgs args)
        // {
        //     if (args.success == false)
        //         ExitAbruptly( $"ModePlay: Failed to join Apian group: \"{args.failureReason}\"");
        // }

        // AppCore event handlers
        private void _OnPlayerJoinedEvt(object sender, PlayerJoinedEventArgs ga)
        {
            bool isLocal = ga.player.PeerId == appl.LocalPeer.PeerId;
            logger.Info($"{(ModeName())} - OnPlayerJoinedEvt() - {(isLocal?"Local":"Remote")} Member Joined: {ga.player.Name}, ID: {SID(ga.player.PeerId)}");
            if (ga.player.PeerId == appl.LocalPeer.PeerId)
            {
                // appCore.RespawnPlayerEvt += _OnRespawnPlayerEvt;  // FIXME: why does this happen here?  &&&&
                // //_SetState(kWaitingForMembers);
                // _SetState(kPlaying);
            }
        }

    }
}



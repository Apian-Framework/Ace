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
                await appl.JoinGameNetworkAsync(settings.apianNetworkName);
                announcedGames = await appl.GetExistingGamesAsync((int)(kListenForGamesSecs*1000));
                GameSelectedEventArgs selection = await appl.SelectGameAsync(announcedGames);
                OnGameSelected(selection);

            } catch (Exception ex) {
                ExitAbruptly( $"{ex.Message}");

                return;
            }
        }

        public void OnGameSelected( GameSelectedEventArgs args)
        {
            AceGameInfo gameInfo = args.gameInfo;
            GameSelectedEventArgs.ReturnCode result = args.result;
            string gameName = gameInfo?.GameName;

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
                }

                switch (result)
                {
                case GameSelectedEventArgs.ReturnCode.kCreate:
                    if (targetGameExisted)
                        ExitAbruptly( $"OnGameSelected(): Cannot create.  Ace Game \"{gameName}\" already exists");
                    else {
                        appl.CreateAndJoinGame(gameInfo, appCore);
                    }
                    break;

                case GameSelectedEventArgs.ReturnCode.kJoin:
                    if (targetGameExisted)
                    {
                        appl.JoinExistingGame(gameInfo, appCore);
                    }
                    else
                        ExitAbruptly( $"OnGameSelected(): Apian Game \"{gameName}\" Not Found");
                    break;
                }
            }
        }

        protected AceAppCore CreateCorePair(AceGameInfo gameInfo)
        {
            // Create gameinstance and ApianInstance
            AceAppCore appCore = new AceAppCore();
            AceApian apian = AceApianFactory.Create(gameInfo.GroupInfo.GroupType, appl.aceGameNet, appCore);
            return appCore;
        }

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



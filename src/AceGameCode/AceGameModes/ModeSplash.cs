using System.Collections.Generic;
using System;
using System.Linq;
using ModalApplication;
using Apian;
using UnityEngine;
using static UniLog.UniLogger; // for SID()

namespace AceGameCode
{
    public class ModeSplash : AceGameMode
    {
        // Mode begins with no AppCore, no Apian, no connection, nothing...
        // This is essentially the same as ModePlay - but creates 2 AI players

        // TODO: factor similar/same stuff out into a parent class
        private const string kNetConnectionString = "p2ploopback";

        private const string kNetworkName = "LocalSplashNet";
        private const string kApianGroupName = "LocalSplashGroup";

        private AceUserSettings settings ;
        private Dictionary<string, AceGameInfo> announcedGames;

        private AceAppCore SplashAppCore; // this is the one we are creating here
        protected const int kTotalPlayers = 2; // No validators
        protected const int kValidatorWaitMs = 0;

		public async override void Start(object param = null)
        {
            base.Start();
            announcedGames = new Dictionary<string, AceGameInfo>();

            settings = appl.frontend.GetUserSettings();
            appl.AddAppCore(null);

            try {
                appl.ConnectToNetwork(kNetConnectionString);
                await appl.JoinGameNetworkAsync(kNetworkName);
                logger.Info("Local splash network joined");

                AceGameInfo gameInfo = appl.aceGameNet.CreateAceGameInfo(
                    kApianGroupName,
                    SinglePeerGroupManager.kGroupType,
                    0,  // min validators
                    0,   // max validators
                    kValidatorWaitMs
                    );

                SplashAppCore = CreateCorePair(gameInfo);
                appl.AddAppCore(SplashAppCore);
                SplashAppCore.PlayerJoinedEvt += _OnPlayerJoinedEvt;
                SplashAppCore.Start(AceCoreModeFactory.kStart );

                LocalPeerJoinedGameData joinData = await  appl.CreateAndJoinGameAsync(gameInfo, SplashAppCore);

            } catch (Exception ex) {
                ExitAbruptly( $"{ex.Message}");

                return;
            }
        }

          // AppCore event handlers
        private void _OnPlayerJoinedEvt(object sender, PlayerJoinedEventArgs ga)
        {
            // Need to create any more?
            if ( SplashAppCore.CoreState.Players.Count < kTotalPlayers)
                appl.SendNewPlayerRequest(SplashAppCore.ApianGroupId, appl.MakeAiAcePlayer());

        }

    }
}



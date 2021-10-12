using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Apian;
using AceGameCode;
using UniLog;

namespace AceCli
{

    public class AceCliFrontend : IAceFrontend
    {
        public IAceApplication AceAppl {get; private set;}
        public IAceAppCore AppCore {get; private set;}

        protected AceUserSettings userSettings;
        public UniLogger logger;

        // Start is called before the first frame update
        public AceCliFrontend(AceUserSettings startupSettings)
        {
            userSettings = startupSettings;
            logger = UniLogger.GetLogger("Frontend");
        }

        public void SetAceApplication(IAceApplication appl)
        {
            AceAppl = appl;
        }

       public void AddAppCore(IAceAppCore core)
        {
            AppCore = core;
            if (core == null)
                return;

            // OnNewCoreState(null, new NewCoreStateEventArgs(core.CoreState)); // initialize

            // core.NewCoreStateEvt += OnNewCoreState;
            // core.PlayerJoinedEvt += OnPlayerJoinedEvt;
            // core.PlayerMissingEvt += OnPlayerMissingEvt;
            // core.PlayerReturnedEvt += OnPlayerReturnedEvt;
            // core.PlayersClearedEvt += OnPlayersClearedEvt;
            // core.NewBikeEvt += OnNewBikeEvt;
            // core.BikeRemovedEvt += OnBikeRemovedEvt;
            // core.BikesClearedEvt +=OnBikesClearedEvt;
            // core.PlaceClaimedEvt += OnPlaceClaimedEvt;
            // core.PlaceHitEvt += OnPlaceHitEvt;

            // core.ReadyToPlayEvt += OnReadyToPlay;

        }

        public virtual void Loop(float frameSecs)
        {

        }

        //
        // IAceFrontend API
        //
        public AceUserSettings GetUserSettings() => userSettings;

        public void DisplayMessage(MessageSeverity lvl, string msgText)
        {
            // Seems like an enum.ToString() is the name of the enum? So this isn't needed,
            // string lvlStr = (lvl == MessageSeverity.Info) ? "Info"
            //     : (lvl == MessageSeverity.Warning) ? "Warning"
            //         : "Error";

            Console.WriteLine($"{lvl}: {msgText}");

            // TODO: should there be a separate HandleUnrecoverableError()
            // API so things like console apps can exit gracfeully? Having it
            // implmented in the FE is a good thing - but it feels a little hokey
            // hanging it onto a DisplayMessage() method
            if (lvl == MessageSeverity.Error)
            {
                AceAppl.ExitApplication();
            }

        }

       // Game code calls with a list of the currently existing games
        // Since this is the CLI app, we mostly ignore that and fetch the "gameName" cli parameter
        // and use that (in a gui App we'd display the list + have a way for the player to
        // enter params for a new game)

        // In the general GUI app case this is an async frontend gui thing
        // and ends with the frontend setting the result for a passed-in TaskCompletionResult

        // In THIS case, we just return (but have to await something to be async)
        public async Task<GameSelectedEventArgs> SelectGameAsync(IDictionary<string, AceGameInfo> existingGames)
        {
            // gameName cli param can end in:
            //  '+' = means join the game if it exists, create if not
            //  '*' = means create if it oes not exist. Error if it's already there
            //  '' = "nothing" means join if it's there, or error
            string gameName = null;
            GameSelectedEventArgs.ReturnCode result;
            AceGameInfo gameInfo;
            int minValidators = 1;
            int validatorWaitMs = 5000;


            string argStr;
            if (userSettings.tempSettings.TryGetValue("gameName", out argStr))
            {
                string groupType;
                if (userSettings.tempSettings.TryGetValue("groupType", out groupType))
                {
                    if (!AceApianFactory.ApianGroupTypes.Contains(groupType))
                        throw new Exception($"Unknown Group Type: {groupType}.");
                    logger.Warn($"Requested group type: {groupType}");
                } else {
                    groupType = CreatorSezGroupManager.kGroupType;
                }


                gameName = argStr.TrimEnd( new [] {'+','*'} );
                result =  (argStr.EndsWith("*")) || (argStr.EndsWith("+") && ! existingGames.Keys.Contains(gameName)) ? GameSelectedEventArgs.ReturnCode.kCreate
                    : GameSelectedEventArgs.ReturnCode.kJoin;

                // TODO: does the frontend have any busniess selecting an agreement type?
                // Hmm. Actually, it kinda does: a user might well want to choose from a set of them.
                gameInfo = existingGames.Keys.Contains(gameName) ? existingGames[gameName]
                    :  AceAppl.aceGameNet.CreateAceGameInfo( gameName, groupType, minValidators, validatorWaitMs);

            }
            else
                throw new Exception($"gameName setting missing.");

            await Task.Delay(0); // Yuk, But usually this is an async UI operation
            return new GameSelectedEventArgs(gameInfo, result);
        }

    }

}

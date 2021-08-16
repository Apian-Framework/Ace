using System.ComponentModel;
using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    }

}

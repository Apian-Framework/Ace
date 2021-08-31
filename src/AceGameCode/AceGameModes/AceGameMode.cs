using System;
using System.Collections.Generic;
using ModalApplication;
using UniLog;

namespace AceGameCode
{
    public class AceGameMode : IAppMode
    {
		public AppModeManager manager;
		public AceApplication appl;

		public UniLogger logger;
		public int ModeId() => manager.CurrentModeId();

		public void Setup(AppModeManager mgr, IModalApp gInst = null)
		{
			// Called by manager before Start()
			// Not virtual
			// TODO: this should be the engine and not the modeMgr - but what IS an engine...
			manager = mgr;
			appl = gInst as AceApplication;
			logger = UniLogger.GetLogger("AceGameMode");
        }

		public virtual void Start( object param = null)	{
            logger.Info($"Starting {(ModeName())}");
        }

		public virtual void Loop(float frameSecs) {}

		public virtual void Pause() {}

		public virtual void Resume( string prevModeName, object param = null)	{
            logger.Info($"Resuming {(ModeName())} from {prevModeName}");
        }

		public virtual object End() => null;
        public virtual string ModeName() => this.GetType().Name;

		public void ExitAbruptly(string message)
		{
			appl.frontend.DisplayMessage(MessageSeverity.Error, message);
			logger.Error(message);
			manager.Stop();
		}
        protected AceAppCore CreateCorePair(AceGameInfo gameInfo)
        {
            // Create gameinstance and ApianInstance
            AceAppCore appCore = new AceAppCore();
            AceApian apian = AceApianFactory.Create(gameInfo.GroupInfo.GroupType, appl.aceGameNet, appCore);
            return appCore;
        }

    }
}
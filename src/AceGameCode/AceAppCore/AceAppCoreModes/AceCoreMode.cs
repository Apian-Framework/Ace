using System;
using System.Collections.Generic;
using ModalApplication;
using UniLog;

namespace AceGameCode
{
    public class AceCoreMode : IAppMode
    {
		public AppModeManager manager;
		public AceApplication appl;
		public UniLogger logger;
		public int ModeId() => manager.CurrentModeId();

		public void Setup(AppModeManager mgr, IModalApp gInst = null)
		{
			// Called by manager before Start()
			// Not virtual
			manager = mgr;
			appl = gInst as AceApplication;
			logger = UniLogger.GetLogger("AceCoreMode");
        }

		public virtual void Start( object param = null)	{
            logger.Info($"Starting {(ModeName())}");
        }

		public virtual void Pause() {}

		public virtual void Resume( string prevModeName, object param = null)	{
            logger.Info($"Resuming {(ModeName())} from {prevModeName}");
        }

		public virtual object End() => null;
        public virtual string ModeName() => this.GetType().Name;

    }
}
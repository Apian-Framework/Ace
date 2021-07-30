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
        public AceAppCore appCore;
        protected AceUserSettings userSettings;
        public UniLogger logger;

        // Start is called before the first frame update
        public AceCliFrontend(AceUserSettings startupSettings)
        {
            userSettings = startupSettings;
            logger = UniLogger.GetLogger("Frontend");
        }

        public void Foo() {} // placeholder. delete me.

        public virtual void Loop(float frameSecs)
        {

        }

        //
        // IAceFrontend API
        //
        public AceUserSettings GetUserSettings() => userSettings;
    }

}

using System.Collections.Generic;
using System;
using System.Linq;
using ModalApplication;
using Apian;
using UnityEngine;

namespace AceGameCode
{
    // Start mode doesn't actually do anything - it's just the initial AppCore mode before anything is connected or being run.

    public class CoreModeStart : AceCoreMode
    {
		public override void Start( object param = null)	{
            base.Start();
        }

    }
}



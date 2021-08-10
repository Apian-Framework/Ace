using System;
using System.Collections.Generic;
using ModalApplication;

namespace AceGameCode
{
    public class AceModeFactory : AppModeFactory
    {
        public const int kPlay = 0;
        public AceModeFactory()
        {
            AppModeCtors =  new Dictionary<int, Func<IAppMode>>  {
                { kPlay, ()=> new ModePlay() },
            };
        }
    }
}
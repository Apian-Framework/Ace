using System;
using System.Collections.Generic;
using ModalApplication;

namespace AceGameCode
{
    public class AceCoreModeFactory : AppModeFactory
    {
        public const int kStart = 0;
        public AceCoreModeFactory()
        {
            AppModeCtors =  new Dictionary<int, Func<IAppMode>>  {
                { kStart, ()=> new CoreModeStart() },
            };
        }
    }
}
using System;
using System.Collections.Generic;
using GameModeMgr;

namespace AceGameCode
{
    public class AceModeFactory : ModeFactory
    {
        public const int kPlay = 0;
        public AceModeFactory()
        {
            ModeFactories =  new Dictionary<int, Func<IGameMode>>  {
                { kPlay, ()=> new ModePlay() },
            };
        }
    }
}
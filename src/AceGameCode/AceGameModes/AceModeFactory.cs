using System;
using System.Collections.Generic;
using ModalApplication;

namespace AceGameCode
{
    public class AceModeFactory : AppModeFactory
    {
        public const int kSplash = 0; // Local loopback P2pNet, Local AI Players
        public const int kPlay = 1; // Real P2pNet, Local + remote players
        public AceModeFactory()
        {
            AppModeCtors =  new Dictionary<int, Func<IAppMode>>  {
                { kSplash, ()=> new ModeSplash() },
                { kPlay, ()=> new ModePlay() },
            };
        }
    }
}
using System;
using Apian;
using P2pNet;

namespace AceGameCode
{
    public interface IAceGameNet : IApianGameNet
    {

    }


    public class AceGameNet : ApianGameNetBase, IAceGameNet
    {

        public AceGameNet() : base()
        {
            logger.Verbose($"Ctor: {this.GetType().Name}");
        }

        protected override IP2pNet P2pNetFactory(string p2pConnectionString)
        {
            // P2pConnectionString is <p2p implmentation name>::<imp-dependent connection string>
            // Names are: p2ploopback, p2predis

            IP2pNet ip2p = null;
            string[] parts = p2pConnectionString.Split(new string[]{"::"},StringSplitOptions.None); // Yikes! This is fugly.

            switch(parts[0])
            {
                // case "p2predis":
                //     ip2p = new P2pRedis(this, parts[1]);
                //     break;
                case "p2ploopback":
                    ip2p = new P2pLoopback(this, null);
                    break;
                // case "p2pactivemq":
                //     p2p = new P2pActiveMq(this, parts[1]);
                //     break;
                default:
                    throw( new Exception($"Invalid connection type: {parts[0]}"));
            }


            return ip2p;
        }

    }
}
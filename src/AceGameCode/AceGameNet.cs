using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Apian;
using P2pNet;
using GameNet;

namespace AceGameCode
{
    public interface IAceGameNet : IApianGameNet
    {
        Task<PeerJoinedNetworkData> JoinGameNetworkAsync(string netName, AceNetworkPeer localPeer);
        AceGameInfo CreateAceGameInfo(string gameName, string apianGroupType);
        void CreateAndJoinGame(AceGameInfo gameInfo, AceApian apian, string localData);
        void JoinExistingGame(AceGameInfo gameInfo, AceApian apian, string localData );

    }


    public class AceGameNet : ApianGameNetBase, IAceGameNet
    {


        public const int kAceNetworkChannelInfo = 0;
        public const int kAceGameChannelInfo = 1;
        public const int kAceChannelInfoCount = 2;

        protected P2pNetChannelInfo[] aceChannelData =  {
            //   name, id, dropMs, pingMs, missingMs, syncMs, maxPeers
            new P2pNetChannelInfo(null, null, 10000, 3000, 5000,     0, 0 ), // Main network channel (no clock sync)
            new P2pNetChannelInfo(null, null, 15000, 4000, 4500, 15000, 0 )  // gameplay channels - should drop from main channel before it happens here
        };

        public AceGameNet() : base()
        {
            logger.Verbose($"Ctor: {this.GetType().Name}");
        }

        public AceGameInfo CreateAceGameInfo(string gameName, string apianGroupType)
        {
           string netName = p2p.GetMainChannel()?.Name;
            if (netName == null)
            {
                logger.Error($"CreateAceGameInfo() - Must join network first"); // TODO: probably ought to assert? Can this be recoverable?
                return null;
            }

            P2pNetChannelInfo groupChanInfo = new P2pNetChannelInfo(aceChannelData[kAceGameChannelInfo]);
            groupChanInfo.name = gameName;
            groupChanInfo.id = $"{netName}/{gameName}";

            ApianGroupInfo groupInfo = new ApianGroupInfo(apianGroupType, groupChanInfo, LocalP2pId(), gameName);

            return new AceGameInfo(groupInfo);
        }

        public void JoinExistingGame(AceGameInfo gameInfo, AceApian apian, string localData )
        {
            string netName = p2p.GetMainChannel()?.Name;
            if (netName == null)
            {
                logger.Error($"JoinExistingGame() - Must join network first"); // TODO: probably ought to assert? Can this be recoverable?
                return;
            }

            base.JoinExistingGroup(gameInfo.GroupInfo, apian, localData);
        }

        public void CreateAndJoinGame(AceGameInfo gameInfo, AceApian apian, string localData)
        {
            string netName = p2p.GetMainChannel()?.Name;
            if (netName == null)
            {
                logger.Error($"CreateAndJoinGame() - Must join network first"); // TODO: probably ought to assert? Can this be recoverable?
                return;
            }

            base.CreateAndJoinGroup(gameInfo.GroupInfo, apian, localData);

        }



        protected override IP2pNet P2pNetFactory(string p2pConnectionString)
        {
            // P2pConnectionString is <p2p implmentation name>::<imp-dependent connection string>
            // Names are: p2ploopback, p2predis

            IP2pNet ip2p = null;
            string[] parts = p2pConnectionString.Split(new string[]{"::"},StringSplitOptions.None); // Yikes! This is fugly.

            switch(parts[0])
            {
                case "p2predis":
                    ip2p = new P2pRedis(this, parts[1]);
                    break;
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

        public async Task<PeerJoinedNetworkData> JoinGameNetworkAsync(string netName, AceNetworkPeer localPeer )
        {
            P2pNetChannelInfo chan = new P2pNetChannelInfo(aceChannelData[kAceNetworkChannelInfo]);
            chan.name = netName;
            chan.id = netName;
            string beamNetworkHelloData = JsonConvert.SerializeObject(localPeer);
            return await JoinNetworkAsync(chan, beamNetworkHelloData);
        }

    }
}
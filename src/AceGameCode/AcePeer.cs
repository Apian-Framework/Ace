using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Apian;
using UnityEngine;

namespace AceGameCode
{
    public class AceNetworkPeer
    {
        public string PeerId { get; private set;}
        public string Name { get; private set;}

        public AceNetworkPeer(string peerId, string name)
        {
            PeerId = peerId;
            Name = name;
        }

        public static AceNetworkPeer FromApianSerialized(string jsonData)
        {
            object[] data = JsonConvert.DeserializeObject<object[]>(jsonData);
            return new AceNetworkPeer(
                data[0] as string,
                data[1] as string);
        }

        public string ApianSerialized()
        {
            return  JsonConvert.SerializeObject(new object[]{
                PeerId,
                Name });
        }

    }
}

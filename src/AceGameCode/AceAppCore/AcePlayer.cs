﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Apian;
using UnityEngine;

namespace AceGameCode
{
    public class AcePlayer : IApianCoreData
    {
        public string PeerId { get; private set;}
        public string Name { get; private set;}

        public AcePlayer(string peerId, string name)
        {
            PeerId = peerId;
            Name = name;
        }

        // Custom compact json
        // TODO: set up params to make more compact.
        public static AcePlayer FromApianJson(string jsonData)
        {
            object[] data = JsonConvert.DeserializeObject<object[]>(jsonData);
            return new AcePlayer(
                data[0] as string,
                data[1] as string);
        }

        public string ApianSerialized(object args=null)
        {
            return  JsonConvert.SerializeObject(new object[]{
                PeerId,
                Name });
        }

    }
}

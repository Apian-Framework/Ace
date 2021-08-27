using System;
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
        public PlayerRole Role { get; private set;}
        public PlaneColor Team { get; private set;}

        public AcePlayer(string peerId, string name, PlayerRole role = PlayerRole.kNone, PlaneColor team = PlaneColor.kNone)
        {
            PeerId = peerId;
            Name = name;
            Role = role;
            Team = team;
        }

        // Custom compact json
        // TODO: set up params to make more compact.
        public static AcePlayer FromApianJson(string jsonData)
        {
            object[] data = JsonConvert.DeserializeObject<object[]>(jsonData);
            return new AcePlayer(
                (string)data[0],
                (string)data[1],
                (PlayerRole)(long)data[2],
                (PlaneColor)(long)data[3]
            );
        }

        public string ApianSerialized(object args=null)
        {
            return  JsonConvert.SerializeObject(new object[]{
                PeerId,
                Name,
                Role,
                Team
            });
        }

    }
}

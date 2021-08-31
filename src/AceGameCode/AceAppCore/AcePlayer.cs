using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Apian;
using UnityEngine;

namespace AceGameCode
{
    public class AcePlayer : IApianCoreData
    {
		public const string NoCtrl = "none";
		public const string RemoteCtrl = "remote";
		public const string AiCtrl = "ai";
		public const string LocalPlayerCtrl = "player";

        public string PlayerId { get; private set;}
        public string Name { get; private set;}
        public string PeerId { get; private set;}
        public string CtrlType { get; private set; }
        public PlayerRole Role { get; private set;}
        public PlaneColor Team { get; private set;}

        public AcePlayer(string playerId, string ctrlType, string name, string peerId,  PlayerRole role, PlaneColor team = PlaneColor.kNone)
        {
            // ColorNone is "I don't care"
            PlayerId = playerId;
            CtrlType = ctrlType;
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
                (string)data[0], //playerId
                (string)data[1], // ctrlTYpe
                (string)data[2], // name
                (string)data[3], // PeerId
                (PlayerRole)(long)data[4], // role
                (PlaneColor)(long)data[5]  // team
            );
        }

        public string ApianSerialized(object args=null)
        {
            return  JsonConvert.SerializeObject(new object[]{
                PlayerId,
                CtrlType,
                Name,
                PeerId,
                Role,
                Team
            });
        }

    }

    // Some utils



}

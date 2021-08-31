using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using AceGameCode;
using UniLog;

namespace AceGameCodeTests
{
    [TestFixture]
    public class AcePlayerTests
    {
        [Test]
        public void AcePlayer_Ctor()
        {
            const string playerId = "aPlayerId";
            const string ctrlType = AcePlayer.AiCtrl;
            const string peerId = "aPeerId";
            const string name = "aName";
            const PlayerRole role = PlayerRole.kPlayer;

            AcePlayer pl = new AcePlayer(playerId, ctrlType, name, peerId, role);

            Assert.That(pl, Is.Not.Null);
            Assert.That(pl.PeerId, Is.EqualTo(peerId));
            Assert.That(pl.CtrlType, Is.EqualTo(ctrlType));
            Assert.That(pl.Name, Is.EqualTo(name));
            Assert.That(pl.Role, Is.EqualTo(role));
            Assert.That(pl.Team, Is.EqualTo(PlaneColor.kNone));
        }

        [Test]
        public void AcePlayer_Serialize()
        {
            const string playerId = "aPlayerId";
            const string ctrlType = AcePlayer.AiCtrl;
            const string peerId = "aPeerId";
            const string name = "aName";
            const PlayerRole role = PlayerRole.kPlayer;

            string expected = "[\"" + playerId + "\",\"" + ctrlType + "\",\"" + name + "\",\""  + peerId + "\"," + (int)role + "," + (int)PlaneColor.kNone + "]";

            AcePlayer pl = new AcePlayer(playerId, ctrlType, name, peerId, role);
            string result = pl.ApianSerialized();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AcePlayer_Deserialize()
        {
            const string playerId = "aPlayerId";
            const string ctrlType = AcePlayer.LocalPlayerCtrl;
            const string peerId = "aPeerId";
            const string name = "aName";
            const PlayerRole role = PlayerRole.kPlayer;

            string json = "[\"" + playerId + "\",\""  + ctrlType + "\",\"" + name +  "\",\"" + peerId + "\"," + (int)role + "," + (int)PlaneColor.kRed + "]";

            AcePlayer pl = AcePlayer.FromApianJson(json);

            Assert.That(pl.PlayerId, Is.EqualTo(playerId));
            Assert.That(pl.PeerId, Is.EqualTo(peerId));
            Assert.That(pl.Name, Is.EqualTo(name));
            Assert.That(pl.Role, Is.EqualTo(role));
            Assert.That(pl.Team, Is.EqualTo(PlaneColor.kRed));
        }

    }

}
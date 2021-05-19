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
            const string peerId = "aPeerId";
            const string name = "aName";

            AcePlayer pl = new AcePlayer(peerId, name);

            Assert.That(pl, Is.Not.Null);
            Assert.That(pl.PeerId, Is.EqualTo(peerId));
            Assert.That(pl.Name, Is.EqualTo(name));
        }

        [Test]
        public void AcePlayer_Serialize()
        {
            const string peerId = "aPeerId";
            const string name = "aName";
            const string expected = "[\"" + peerId + "\",\"" + name + "\"]";

            AcePlayer pl = new AcePlayer(peerId, name);
            string result = pl.ApianSerialized();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AcePlayer_Deserialize()
        {
            const string peerId = "aPeerId";
            const string name = "aName";
            const string json = "[\"" + peerId + "\",\"" + name + "\"]";

            AcePlayer pl = AcePlayer.FromApianJson(json);

            Assert.That(pl.PeerId, Is.EqualTo(peerId));
            Assert.That(pl.Name, Is.EqualTo(name));
        }

    }

}
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
    public class AceCoreStateTests
    {
        [Test]
        public void AceCoreState_Ctor()
        {
            AceCoreState cs = new AceCoreState();

            Assert.That(cs, Is.Not.Null);
            Assert.That(cs.Logger, Is.InstanceOf<UniLogger>());
            Assert.That(cs.PlayersById, Is.InstanceOf<Dictionary<string,AcePlayer>>());
            Assert.That(cs.PlayersById.Count, Is.EqualTo(0));
        }
    }

}
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using AceGameCode;
using UnityEngine;
using UniLog;

namespace AceGameCodeTests
{
    [TestFixture]
    public class AcePlaneTests
    {
        [Test]
        public void AcePlane_Ctor()
        {
            const string planeId = "aPlaneId";
            const string playerId = "aPeerId";
            const string name = "aName";
            const string ctrl = "ai";
            Vector2 pos = new Vector2( 3, 7);
            PlaneOrientation orientation = PlaneOrientation.kUp;

            AcePlane pl = new AcePlane(planeId, playerId, name, ctrl, pos, orientation);

            Assert.That(pl, Is.Not.Null);
            Assert.That(pl.PlaneId, Is.EqualTo(planeId));
            Assert.That(pl.PlayerId, Is.EqualTo(playerId));
            Assert.That(pl.Name, Is.EqualTo(name));
            Assert.That(pl.CtrlType, Is.EqualTo(ctrl));
            Assert.That(pl.Position, Is.EqualTo(pos));
            Assert.That(pl.Orientation, Is.EqualTo(orientation));
        }

        [Test]
        public void AcePlane_Serialize()
        {
            const string planeId = "xPlaneId";
            const string playerId = "yPeerId";
            const string name = "zName";
            const string ctrl = "ai";
            Vector2 pos = new Vector2(4,3);
            PlaneOrientation orientation = PlaneOrientation.kConcealed;

            string expected = $"[\"{planeId}\",\"{playerId}\",\"{name}\",\"{ctrl}\",{pos.x},{pos.y},{(int)orientation}]";

            AcePlane pl = new AcePlane(planeId, playerId, name, ctrl, pos, orientation);
            string result = pl.ApianSerialized();

            Assert.That(result, Is.EqualTo(expected));

        }

        [Test]
        public void AcePlane_Deserialize()
        {
            const string planeId = "xPlaneId";
            const string playerId = "yPeerId";
            const string name = "zName";
            const string ctrl = "ai";
            Vector2 pos = new Vector2(4,3);
            PlaneOrientation orientation = PlaneOrientation.kConcealed;
            string json = $"[\"{planeId}\",\"{playerId}\",\"{name}\",\"{ctrl}\",{pos.x},{pos.y},{(int)orientation}]";

            AcePlane pl =  AcePlane.FromApianJson(json);

            Assert.That(pl.PlaneId, Is.EqualTo(planeId));
            Assert.That(pl.Position, Is.EqualTo(pos));


        }

    }

}
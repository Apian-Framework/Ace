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
    public class AceBoardTests
    {
        [Test]
        public void AceBoard_Ctor()
        {
            const int defW = 6, testW = 7;
            const int defH = 5, testH = 9;

            AceBoard pl = new AceBoard();

            Assert.That(pl, Is.Not.Null);
            Assert.That(pl.Width, Is.EqualTo(defW));
            Assert.That(pl.Height, Is.EqualTo(defH));

            AceBoard p2 = new AceBoard(testW, testH);

            Assert.That(p2, Is.Not.Null);
            Assert.That(p2.Width, Is.EqualTo(testW));
            Assert.That(p2.Height, Is.EqualTo(testH));
        }

        [Test]
        public void AceBoard_Serialize()
        {
            const int testW = 7;
            const int testH = 9;
            string expected = $"[{testW},{testH}]";

            AceBoard bl = new AceBoard(testW, testH);
            string result = bl.ApianSerialized();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AceBoard_Deserialize()
        {
            const int testW = 7;
            const int testH = 9;
            string json = $"[{testW},{testH}]";

            AceBoard bl = AceBoard.FromApianJson(json);

            Assert.That(bl.Width, Is.EqualTo(testW));
            Assert.That(bl.Height, Is.EqualTo(testH));
        }

    }

}
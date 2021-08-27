using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Moq.Protected;
using AceCli;
using UniLog;
using AceGameCode;
using System.Reflection;

namespace AceCliTests
{
    [TestFixture]


    public class CliDriverTests
    {
        public class CliDriverWrapper : CliDriver
        {
            public void CallInit(AceUserSettings bus )
            {
                base.Init(bus);
            }
        }

        [Test]
        public void CliDriver_ConstructorWorks()
        {
            CliDriver drv = new CliDriver();
            long tfms = drv.targetFrameMs;

            Assert.That(drv, Is.Not.Null);
            Assert.That(tfms, Is.EqualTo(250)); // FIXME - this is a silly and fragile test
        }

        [Test]
        public void CliDriver_Init()
        {
            Mock<AceUserSettings> settings = new Mock<AceUserSettings>();

            CliDriverWrapper drvw = new CliDriverWrapper();
            drvw.CallInit(settings.Object);

            // Assert.That(drvw.fe, Is.InstanceOf<AceCliFrontend>());
            // Assert.That(drvw.gn, Is.InstanceOf<AceGameNet>());
            // Assert.That(drvw.appl, Is.InstanceOf<AceApplication>());
            // Assert.That(drvw.fe.aceAppl, Is.EqualTo(drvw.appl));
        }
    }

    // [TestFixture]

    // public class CliProgramTests
    // {
    //     public object SettingByName(AceUserSettings s, string name )
    //     {
    //        Type myType = typeof(AceUserSettings);
    //        FieldInfo fi = myType.GetField(name);
    //        return fi.GetValue(s);
    //     }

    //     public class WrappedProgram : Program
    //     {
    //         public static AceUserSettings DoGetSettings(string[] args)
    //         {
    //             return GetSettings(args);
    //         }
    //     }

    //     [TestCase("--startmode,2", "startMode", 2)]
    //     [TestCase("--bikectrl,ai", "localPlayerCtrlType", "ai")]
    //     [TestCase("--defloglvl,info", "defaultLogLevel", "info")]
    //     [TestCase("--throwonerror,true", "DefaultThrowOnError", true)]
    //     [TestCase("--gamename,bar+", "gameName", "bar+")]
    //     public void CliProgram_GetSettings(string argsString, string settingName, object val)
    //     {
    //         var args = argsString.Split(',');
    //         AceUserSettings sets = WrappedProgram.DoGetSettings(args);
    //         Assert.That(sets.version, Is.EqualTo(UserSettingsMgr.currentVersion));

    //         switch(settingName)
    //         {
    //         case "gameName":
    //             Assert.That(sets.tempSettings["gameName"], Is.EqualTo(val));
    //             break;
    //         case "DefaultThrowOnError":
    //             Assert.That(UniLogger.DefaultThrowOnError, Is.EqualTo(val));
    //             break;
    //         default:
    //             Assert.That(SettingByName(sets, settingName), Is.EqualTo(val));
    //             break;
    //         }

    //     }

    // }

}
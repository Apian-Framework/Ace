using System;
using System.Diagnostics;
using System.Threading;
using CommandLine;
using AceGameCode;
using UniLog;

namespace AceCli
{
    public static class Program
    {
        public class CliOptions
        {
            [Option(
	            Default = null,
	            HelpText = "Join this game. Else create a game")]
            public string GameName {get; set;}

            [Option(
	            Default = null,
	            HelpText = "Apian Network name" )]
            public string NetName {get; set;}

            [Option(
	            Default = null,
	            HelpText = "Apian Group consensus mechanism (if creating)" )]
            public string GroupType {get; set;}

            [Option(
	            Default = -1,
	            HelpText = "Start with this GameMode" )]
            public int StartMode {get; set;}

            [Option(
	            Default = null,
	            HelpText = "User settings file basename (Default: beamsettings)")]
            public string Settings {get; set;}

            [Option(
	            Default = false,
	            HelpText = "Force default user settings (other than CLI options")]
            public bool ForceDefaultSettings {get; set;}

            [Option(
	            Default = null,
	            HelpText = "(Default: Warn) Default log level.")]
            public string DefLogLvl {get; set;}

            [Option(
	            Default = false,
	            HelpText = "Raise exception on Unilog error")]
            public bool ThrowOnError {get; set;}
        }

        public static AceUserSettings GetSettings(string[] args)
        {
            AceUserSettings settings = UserSettingsMgr.Load();

                Parser.Default.ParseArguments<CliOptions>(args)
                    .WithParsed<CliOptions>(o =>
                    {
                        if (o.Settings != null)
                            settings = UserSettingsMgr.Load(o.Settings);

                        if (o.ForceDefaultSettings)
                            settings = AceUserSettings.CreateDefault();

                        if (o.ThrowOnError)
                            UniLogger.DefaultThrowOnError = true;

                        if (o.DefLogLvl != null)
                            settings.defaultLogLevel = o.DefLogLvl;

                        if (o.NetName != null)
                            settings.apianNetworkName = o.NetName;

                        if (o.GameName != null)
                            settings.tempSettings["gameName"] = o.GameName;

                        if (o.GroupType != null)
                            settings.tempSettings["groupType"] = o.GroupType;

                       if (o.StartMode != -1)
                            settings.startMode = o.StartMode;

                    }).WithNotParsed(o =>
                    {
                        // --help, --version, or any error results in this getting called
                        settings = null;
                    });

            if (settings != null)
                UserSettingsMgr.Save(settings);
            return settings;
        }

        static void Main(string[] args)
        {
            AceUserSettings settings = GetSettings(args);
            UniLogger.SetupLevels(settings.logLevels);
            CliDriver drv = new CliDriver();
            drv.Run(settings);
        }
    }


    public class CliDriver
    {
        public long targetFrameMs {get; private set;} = 250; // FIXME: should just be for frontend

        public AceApplication appl;
        public AceCliFrontend fe;
        public AceGameNet gn;

        public UniLogger Logger {get; private set;}

        public void Run(AceUserSettings settings) {
            Init(settings);
            LoopUntilDone();
        }

        protected void Init(AceUserSettings settings)
        {
            Logger = UniLogger.GetLogger("CliDriver");
            fe = new AceCliFrontend(settings);
            gn = new AceGameNet();
            appl = new AceApplication(gn, fe);
            appl.Start(settings.startMode);
        }

        protected void LoopUntilDone()
        {
            bool keepRunning = true;
            long frameStartMs = _TimeMs() - targetFrameMs;;
            while (keepRunning)
            {
                long prevFrameStartMs = frameStartMs;
                frameStartMs = _TimeMs();

                // call loop
                keepRunning = Loop((int)(frameStartMs - prevFrameStartMs));
                long elapsedMs = _TimeMs() - frameStartMs;

                // wait to maintain desired rate
                int waitMs = (int)(targetFrameMs - elapsedMs);
                Logger.Debug($"Elapsed ms: {elapsedMs}, Wait ms: {waitMs}");
                if (waitMs <= 0)
                    waitMs = 1;
                Thread.Sleep(waitMs);
            }
        }

        protected bool Loop(int frameMs)
        {
            Logger.Debug($"Loop( {frameMs} )");
            float frameSecs = (float)frameMs / 1000f;
            gn.Update();
            // bool keepRunning = appl.Loop(frameSecs); // Do game code loop
            fe.Loop(frameSecs);
            return appl.IsRunning;
       }

        private static long _TimeMs() =>  DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

    }
}


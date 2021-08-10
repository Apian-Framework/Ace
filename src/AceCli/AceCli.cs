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
	            HelpText = "Join this game")]
            public string GameSpec {get; set;}

            [Option(
	            Default = -1,
	            HelpText = "(persistent) Start with this GameMode")]
            public int StartMode {get; set;}

            [Option(
	            Default = null,
	            HelpText = "User settings basename (Default: acesettings)")]
            public string Settings {get; set;}

            [Option(
	            Default = false,
	            HelpText = "Force default user settings (other than CLI options")]
            public bool ForceDefaultSettings {get; set;}

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

                        if (o.GameSpec != null)
                            settings.tempSettings["gameSpec"] = o.GameSpec;

                       if (o.StartMode != -1)
                            settings.startMode = o.StartMode;

                    });

            UserSettingsMgr.Save(settings);
            return settings;
        }

        static void Main(string[] args)
        {
            AceUserSettings settings = GetSettings(args);
            UniLogger.SetupLevels(settings.debugLevels);
            CliDriver drv = new CliDriver();
            drv.Run(settings);
        }
    }


    public class CliDriver
    {
        public long targetFrameMs {get; private set;} = 16;

        public AceApplication appl;
        public AceCliFrontend fe;
        public AceGameNet gn;


        public void Run(AceUserSettings settings) {
            Init(settings);
            LoopUntilDone();
        }

        protected void Init(AceUserSettings settings)
        {
            // fe = new AceCliFrontend(settings);
            // gn = new AceGameNet();
            // appl = new AceApplication(gn, fe);
            // appl.Start(settings.startMode);
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
                //UnityEngine.Debug.Log(string.Format("Elapsed ms: {0}, Wait ms: {1}",elapsedMs, waitMs));
                if (waitMs <= 0)
                    waitMs = 1;
                Thread.Sleep(waitMs);
            }
        }

        protected bool Loop(int frameMs)
        {
            // first dispatch incoming messages
            // while not self.netCmdQueue.empty():
            //     cmd = self.netCmdQueue.get(block=False)
            //     if cmd:
            //         self._dispatch_net_cmd(cmd)
            //         ge_sleep(0)  # yield


            // while not self.feMsgQueue.empty():
            //     cmd = self.feMsgQueue.get(block=False)
            //     if cmd:
            //         self._dispatch_fe_cmd(cmd)
            //         ge_sleep(0)  # yield

            // then update the game

            // float frameSecs = (float)frameMs / 1000f;
            // gn.Loop();
            // fe.Loop(frameSecs);
            // return appl.Loop(frameSecs);

            return false;
        }

        private static long _TimeMs() =>  DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

    }
}


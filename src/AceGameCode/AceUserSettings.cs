using System.Runtime.Serialization;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UniLog;

namespace AceGameCode
{
    public static class UserSettingsMgr
    {
        public const string currentVersion = "100";
        public const string subFolder = ".ace";
        public const string defaultBaseName= "acesettings";
        public static string fileBaseName;
        public static string path  = GetPath(subFolder);

        public static AceUserSettings Load(string baseName = defaultBaseName)
        {
            fileBaseName = baseName;
            AceUserSettings settings;
            string filePath = path + Path.DirectorySeparatorChar + fileBaseName + ".json";
            try {
                settings = JsonConvert.DeserializeObject<AceUserSettings>(File.ReadAllText(filePath));
            } catch(Exception) {
                settings =  AceUserSettings.CreateDefault();
            }

            // TODO: in real life this should do at least 1 version's worth of updating.
            if (settings.version != currentVersion)
                throw( new Exception($"Invalid settings version: {settings.version}"));

            return settings;
        }

        public static void Save(AceUserSettings settings)
        {
            System.IO.Directory.CreateDirectory(path);
            string filePath = path + Path.DirectorySeparatorChar + fileBaseName + ".json";
            AceUserSettings saveSettings = new AceUserSettings(settings);
            saveSettings.tempSettings = new Dictionary<string, string>(); // Don't persist temp settings
            File.WriteAllText(filePath, JsonConvert.SerializeObject(saveSettings, Formatting.Indented));
        }

        public static string GetPath(string leafFolder)
        {
#if UNITY_2019_1_OR_NEWER
            string homePath =  Application.persistentDataPath;

#else
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                        Environment.OSVersion.Platform == PlatformID.MacOSX)
                        ? Environment.GetEnvironmentVariable("HOME")
                        : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
#endif
            UniLogger.GetLogger("UserSettings").Info($"User settings path: {homePath + Path.DirectorySeparatorChar + leafFolder}");
            return homePath + Path.DirectorySeparatorChar + leafFolder;
        }

    }


    public class AceUserSettings
    {
        public string version = UserSettingsMgr.currentVersion;
        public int startMode;
        public string screenName;
        public string p2pConnectionString;
        public string apianNetworkName;
        public string ethNodeUrl;
        public string ethAcct;
        public string defaultLogLevel;

        public Dictionary<string, string> logLevels;

        // Ace tempSettings include:
        // gameName
        // groupType
        // validator - overrides player creation
        public Dictionary<string, string> tempSettings; // dict of cli-set, non-peristent values
        public Dictionary<string, string> platformSettings; // dict of persistent, but platform-specific, settings

        public AceUserSettings()
        {
            logLevels = new Dictionary<string, string>();
            tempSettings = new Dictionary<string, string>();
            platformSettings = new Dictionary<string, string>();
        }

        public AceUserSettings(AceUserSettings source)
        {
            if (version != source.version)
                throw( new Exception($"Invalid settings version: {source.version}"));
            startMode = source.startMode;
            screenName = source.screenName;
            p2pConnectionString = source.p2pConnectionString;
            apianNetworkName = source.apianNetworkName;
            ethNodeUrl = source.ethNodeUrl;
            ethAcct = source.ethAcct;
            defaultLogLevel = source.defaultLogLevel;
            logLevels = source.logLevels ?? new Dictionary<string, string>();
            tempSettings = source.tempSettings ?? new Dictionary<string, string>();
            platformSettings = source.platformSettings ?? new Dictionary<string, string>();
        }

        public static AceUserSettings CreateDefault()
        {
            return new AceUserSettings() {
                version = UserSettingsMgr.currentVersion,
                screenName = "Fred Sanford",
                p2pConnectionString = "p2predis::newsweasel.com,password=O98nfRVWYYHg7rXpygBCBZWl+znRATaRXTC469SafZU",
                apianNetworkName = "AceNet1",
                ethNodeUrl = "https://rinkeby.infura.io/v3/7653fb1ed226443c98ce85d402299735",
                ethAcct = "0x2b42eBD222B5a1134e85D78613078740eE3Cc93D",
                defaultLogLevel = "Warn",
                logLevels = new Dictionary<string, string>() {
                    {"UserSettings", UniLogger.LevelNames[UniLogger.Level.Info]},
                    {"GameInstance", UniLogger.LevelNames[UniLogger.Level.Warn]},
                },
                tempSettings = new Dictionary<string, string>(),
                platformSettings = new Dictionary<string, string>()
            };
        }
    }
}
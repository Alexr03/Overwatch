using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Alexr03.Overwatch
{
    [Serializable]
    public class Report
    {
        public ReportMeta Meta;

        public Guid ApiKey;

        public string Game = "Unturned";

        public string Reporter;

        public ulong Suspect;

        public string Reason;

        public Report()
        {
        }

        public Report(Guid apiKey, string reporter, ReportMeta meta, ulong suspect, string reason)
        {
            this.ApiKey = apiKey;
            this.Meta = meta;
            this.Reporter = reporter;
            this.Suspect = suspect;
            this.Reason = reason;
        }
    }

    [Serializable]
    public class ReportMeta
    {
        public string Instance;

        public ushort Port;

        public string Map;

        public string SteamName;

        public string DisplayName;

        public ulong GroupID;

        public string Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public ReportMeta()
        {
        }

        public ReportMeta(string steamName, string displayName, ulong groupID, string instance, string map, ushort port)
        {
            this.SteamName = steamName;
            this.DisplayName = displayName;
            this.GroupID = groupID;
            this.Instance = instance;
            this.Map = map;
            this.Port = port;
        }
    }

    [Serializable]
    public class Sample
    {
        public SampleMeta Meta;

        public byte[] Screenshot;

        public Sample()
        {
        }

        public Sample(SampleMeta meta, byte[] screenshot, string suspect, Guid apiKey)
        {
            this.Meta = meta;
            this.Screenshot = screenshot;
        }
    }

    public class SampleMeta
    {

        public byte Health;

        public string Stance;

        public string Vision = "NONE";

        public float Time;

        public SampleMeta()
        {
        }

        public SampleMeta(byte health, string stance, string vision, float time)
        {
            this.Health = health;
            this.Stance = stance;
            this.Vision = vision;
            this.Time = time;
        }
    }
}

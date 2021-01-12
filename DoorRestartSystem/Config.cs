using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DoorRestartSystem
{
    public sealed class Config : IConfig
    {
        [Description("Enable or disable CustomDoorAccess.")]
        public bool IsEnabled { get; set; } = true;

        [Description("The InitialDelay before the first doorrestart can happen")]
        public float InitialDelay { get; set; } = 120f;

        [Description("The Minumum Duration of the Lockdown")]
        public float DurationMin { get; set; } = 5f;

        [Description("The Maximum Duration of the Lockdown")]
        public float DurationMax { get; set; } = 15f;

        [Description("The The Minumum Delay before the next the Lockdown")]
        public float DelayMin { get; set; } = 300f;

        [Description("The The Maxiumum Delay before the next the Lockdown")]
        public float DelayMax { get; set; } = 500f;

        [Description("DoorRestartSencent")]
        public string DoorSentence { get; set; } = "WARNING . DOOR SOFTWARE REPAIR IN t minus 20 seconds.";

        [Description("The time between the sentence and the 3 . 2 . 1 announcement")]
        public float timebtweensntnstart { get; set; } = 17f;

        [Description("DoorAfterRestartSencent")]
        public string DoorAfterSentence { get; set; } = "DOOR SOFTWARE REPAIR COMPLETE";

       

    }
}

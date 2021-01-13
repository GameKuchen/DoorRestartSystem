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

        [Description("The sentence it trasnmits via Cassie before the System gets restarted")]
        public string DoorSentence { get; set; } = "pitch_0.2 .g4 . .g4 pitch_1 door control system pitch_0.25 .g1 pitch_0.9 malfunction pitch_1 . initializing repair";

        [Description("Enable 3 . 2 . 1 announcement")]
        public bool countdown { get; set; } = true;

        [Description("The time between the sentence and the 3 . 2 . 1 announcement")]
        public float timebtweensntnstart { get; set; } = 17f;

        [Description("DoorAfterRestartSencent")]
        public string DoorAfterSentence { get; set; } = "DOOR CONTROL SYSTEM REPAIR COMPLETE";

       

    }
}

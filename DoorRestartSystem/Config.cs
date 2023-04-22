using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DoorRestartSystem2
{
    public class Config : IConfig
    {
        [Description("Enable or disable DoorRestartSystem.")]
        public bool IsEnabled { get; set; } = true;
        
        [Description("Enables debugging.")]
        public bool Debug { get; set; } = false;

        [Description("The InitialDelay before the first Door Restart can happen")]
        public float InitialDelay { get; set; } = 120f;

        [Description("The Minumum Duration of the Lockdown")]
        public float DurationMin { get; set; } = 5f;

        [Description("The Maximum Duration of the Lockdown")]
        public float DurationMax { get; set; } = 15f;

        [Description("The The Minumum Delay before the next the Lockdown")]
        public float DelayMin { get; set; } = 300f;

        [Description("The The Maxiumum Delay before the next the Lockdown")]
        public float DelayMax { get; set; } = 500f;

        [Description("The chance that a Round even has Doorsystemrestarts")]
        public float Spawnchance { get; set; } = 45;

        [Description("The sentence it transmits via Cassie before the System gets restarted")]
        public string DoorSentence { get; set; } = "pitch_0.2 .g4 . .g4 pitch_1 door control system pitch_0.25 .g1 pitch_0.9 malfunction pitch_1 . initializing repair";

        [Description("Enable 3 . 2 . 1 announcement")]
        public bool Countdown { get; set; } = false;

        [Description("The time between the sentence and the 3 . 2 . 1 announcement")]
        public float TimeBetweenSentenceAndStart { get; set; } = 12f;

        [Description("The sentence it transmits via Cassie after the system got restarted")]
        public string DoorAfterSentence { get; set; } = "DOOR CONTROL SYSTEM REPAIR COMPLETE";

        [Description("Should doors close during lockdown (NOTE: This will effect 914 machine doors, use with caution!)")]
        public bool CloseDoors { get; set; } = false;
    }
}

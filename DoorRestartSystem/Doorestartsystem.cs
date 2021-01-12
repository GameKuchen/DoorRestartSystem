using System;
using System.Collections;
using System.Collections.Generic;
using DoorRestartSystem.Handlers;
using Exiled.API.Enums;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using MEC;



namespace DoorRestartSystem
{

    public class Doorestartsystem : Plugin<Config>
    {
        private static readonly Lazy<Doorestartsystem> LazyInstance = new Lazy<Doorestartsystem>(() => new Doorestartsystem());
        public Doorestartsystem Instance => LazyInstance.Value;
        public Random Gen = new Random();
        private Handlers.Server server;
        private Handlers.Player player;

        public NineTailedFoxAnnouncer Respawn;

        public static bool TimerOn = true;
        
        public float DurationMin;
        public float DurationMax;
        public static int DelayMax;
        System.Random random = new System.Random();
        public static int DelayMin;

        public override PluginPriority Priority { get; } = PluginPriority.Medium;
        public override void OnEnabled()
        {
            base.OnEnabled();
            Log.Info("Reached OnEnabled");
            registerEvents();

        }

        private void registerEvents()
        {
            server = new Handlers.Server(this);
            player = new Handlers.Player(this);
            Exiled.Events.Handlers.Server.RoundStarted += server.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += server.OnRoundEnd;
            Exiled.Events.Handlers.Server.WaitingForPlayers += server.OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.TriggeringTesla += player.OnTriggerTesla;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            foreach (CoroutineHandle handle in server.Coroutines)
                Timing.KillCoroutines(handle);
        }



       
        public Doorestartsystem()
        {
        }

        public IEnumerator<float> RunBlackoutTimer()
        {
            Log.Info("Reached BlackoutTimer");
            if (Respawn == null)
                Respawn = PlayerManager.localPlayer.GetComponent<NineTailedFoxAnnouncer>();
            yield
                return Timing.WaitForSeconds((float) random.NextDouble() * (Config.DurationMax - Config.DurationMin) + Config.DurationMin);

            for(; ; )
            {
                if(Warhead.IsDetonated || Warhead.IsInProgress)
                {
                    yield return 0;
                }
                Cassie.Message(Config.DoorSentence, false, true);

                TimerOn = true;
                yield
                return Timing.WaitForSeconds(Config.timebtweensntnstart);
                Cassie.Message("3 . 2 . 1", false, true);
                yield
                return Timing.WaitForSeconds(3f);
                float BlackoutDur = (float)(random.NextDouble() * (Config.DurationMax - Config.DurationMin) + Config.DurationMin);


                foreach (DoorVariant door in Map.Doors)
                {
                    door.NetworkTargetState = false;
                    door.NetworkActiveLocks = 1;
                }
                yield
                return Timing.WaitForSeconds(BlackoutDur);
                foreach (DoorVariant door in Map.Doors)
                {
                    door.NetworkActiveLocks = 0;
                }
                Cassie.Message(Config.DoorAfterSentence, false, true);
                yield return Timing.WaitForSeconds((float)random.NextDouble() * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
                TimerOn = false;
            }
        }
    }



        
        

}

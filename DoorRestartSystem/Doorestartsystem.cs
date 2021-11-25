using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Extensions;
using Interactables.Interobjects.DoorUtils;
using MEC;

namespace DoorRestartSystem
{

    public class DoorRestartSystem : Plugin<Config>
    {
        public override string Author => "GameKuchen";
        public override string Name => "DoorRestartSystem";
        public override string Prefix => "DRS";
        public override Version Version => new Version(2, 7, 0);
        public override Version RequiredExiledVersion => new Version(2, 1, 30);
        public Random Gen = new Random();
        public static DoorRestartSystem Singleton;
        private Handlers.Server server;
        private Handlers.Player player;
        public NineTailedFoxAnnouncer Respawn;
        public static bool TimerOn = true;
        System.Random random = new System.Random();
        public override PluginPriority Priority => PluginPriority.Medium;

        public override void OnEnabled()
        {
            Singleton = this;
            registerEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(server.Coroutine);
            unRegisterEvents();
            base.OnDisabled();
        }

        public static void softlockDoors()
        {
            foreach (Door door in Map.Doors)
            {
                door.ChangeLock(DoorLockType.Warhead);
            }
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

        private void unRegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= server.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= server.OnRoundEnd;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= server.OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.TriggeringTesla -= player.OnTriggerTesla;

            server = null;
            player = null;
        }

        public IEnumerator<float> RunBlackoutTimer()
        {
            yield return Timing.WaitForSeconds(Config.InitialDelay);
            yield return Timing.WaitForSeconds((float)random.NextDouble() * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
            for (; ; )
            {
                yield return Timing.WaitUntilTrue(() => !(Warhead.IsDetonated || Warhead.IsInProgress));
                Cassie.Message(Config.DoorSentence, false, true);

                TimerOn = true;
                yield return Timing.WaitForSeconds(Config.TimeBetweenSentenceAndStart);
                if (Config.Countdown)
                {
                    Cassie.Message("3 . 2 . 1", false, true);
                    yield return Timing.WaitForSeconds(3f);
                }

                float BlackoutDur = (float)(random.NextDouble() * (Config.DurationMax - Config.DurationMin) + Config.DurationMin);


                foreach (Door door in Map.Doors)
                {
                    if (door.Type != DoorType.NukeSurface)
                    {
                        door.ChangeLock(DoorLockType.SpecialDoorFeature);
                    }
                }
                yield return Timing.WaitForSeconds(BlackoutDur);
                foreach (Door door in Map.Doors)
                {
                    door.ChangeLock(DoorLockType.SpecialDoorFeature);
                }
                Cassie.Message(Config.DoorAfterSentence, false, true);
                yield return Timing.WaitForSeconds((float)random.NextDouble() * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
                TimerOn = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Extensions;
using Interactables.Interobjects.DoorUtils;
using MEC;
using UnityEngine;
using Random = System.Random;

namespace DoorRestartSystem2
{

    public class DoorRestartSystemNew : Plugin<Config>
    {
        public override string Author => "GameKuchen";
        public override string Name => "DoorRestartSystem";
        public override string Prefix => "DRS";
        public override Version Version => new Version(3, 5, 0);
        public override Version RequiredExiledVersion => new Version(5, 0, 0);
        public Random Gen = new Random();
        private static DoorRestartSystemNew _singleton;
        private Handlers.Server _server;
        public NineTailedFoxAnnouncer Respawn;
        private static bool _timerOn = true;
        private Random Random { get; } = new Random();
        public override PluginPriority Priority => PluginPriority.Medium;

        public override void OnEnabled()
        {
            _singleton = this;
            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(_server.Coroutine);
            UnRegisterEvents();
            base.OnDisabled();
        }

        public static void SoftlockDoors()
        {
            foreach (var door in Door.List)
            {
                door.ChangeLock(DoorLockType.Warhead);
            }
        }

        private void RegisterEvents()
        {
            _server = new Handlers.Server(this);
            Exiled.Events.Handlers.Server.RoundStarted += _server.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += _server.OnRoundEnd;
            Exiled.Events.Handlers.Server.WaitingForPlayers += _server.OnWaitingForPlayers;
        }

        private void UnRegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= _server.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= _server.OnRoundEnd;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= _server.OnWaitingForPlayers;

            _server = null;
        }

        public IEnumerator<float> RunBlackoutTimer()
        {
            yield return Timing.WaitForSeconds(Config.InitialDelay);
            yield return Timing.WaitForSeconds((float)Random.NextDouble() * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
            for (; ; )
            {
                yield return Timing.WaitUntilTrue(() => !(Warhead.IsDetonated || Warhead.IsInProgress));
                Cassie.Message(Config.DoorSentence);

                _timerOn = true;
                yield return Timing.WaitForSeconds(Config.TimeBetweenSentenceAndStart);
                if (Config.Countdown)
                {
                    Cassie.Message("3 . 2 . 1");
                    yield return Timing.WaitForSeconds(3f);
                }

                var blackoutDur = (float)(Random.NextDouble() * (Config.DurationMax - Config.DurationMin) + Config.DurationMin);


                foreach (var door in Door.List)
                {
                    if (door.Type == DoorType.NukeSurface) continue;
                    if (Config.CloseDoors)
                        door.IsOpen = false;
                    door.ChangeLock(DoorLockType.SpecialDoorFeature);
                }
                yield return Timing.WaitForSeconds(blackoutDur);
                foreach (var door in Door.List)
                {
                    if(door.Type == DoorType.NukeSurface) continue;
                    door.ChangeLock(DoorLockType.SpecialDoorFeature);
                }
                Cassie.Message(Config.DoorAfterSentence);
                yield return Timing.WaitForSeconds((float)Random.NextDouble() * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
                _timerOn = false;
            }
        }
    }
}

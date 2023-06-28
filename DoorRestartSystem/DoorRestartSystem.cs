using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using Server = DoorRestartSystem.Handlers.Server;

namespace DoorRestartSystem
{

    public class DoorRestartSystemNew : Plugin<Config>
    {
        public override string Author => "GameKuchen";
        public override string Name => "DoorRestartSystem";
        public override string Prefix => "DRS";
        public override Version Version => new Version(3, 6, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0);
        private Server _server;
        private static bool _timerOn = true;
        public override PluginPriority Priority => PluginPriority.Medium;

        public override void OnEnabled()
        {
            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(_server.Coroutine);
            UnRegisterEvents();
            base.OnDisabled();
        }
        

        private void RegisterEvents()
        {
            _server = new Server(this);
            Exiled.Events.Handlers.Server.RoundStarted += _server.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += _server.OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers += _server.OnWaitingForPlayers;
        }

        private void UnRegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= _server.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= _server.OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= _server.OnWaitingForPlayers;

            _server = null;
        }

        public IEnumerator<float> RunBlackoutTimer()
        {
            yield return Timing.WaitForSeconds(Config.InitialDelay);
            yield return Timing.WaitForSeconds(UnityEngine.Random.value * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
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

                var blackoutDur = UnityEngine.Random.value * (Config.DurationMax - Config.DurationMin) + Config.DurationMin;


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
                yield return Timing.WaitForSeconds(UnityEngine.Random.value * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
                _timerOn = false;
            }
        }
    }
}

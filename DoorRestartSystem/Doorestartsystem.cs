using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using MEC;

namespace DoorRestartSystem
{ 
    public class DoorRestartSystem : Plugin<Config>
    {
        public override string Author => "GameKuchen";
        public override string Name => "DoorRestartSystem";
        public override string Prefix => "DRS";
        public override Version Version => new Version(3, 0, 1);
        public override Version RequiredExiledVersion => new Version(3, 0, 0);
        public Random Gen { get; } = new Random();
        public static DoorRestartSystem Singleton;
        private Handlers.Server _server;
        private Handlers.Player _player;
        public static bool TimerOn = true;
        private System.Random Random { get; }= new System.Random();
        public override PluginPriority Priority => PluginPriority.Medium;

        public override void OnEnabled()
        {
            Singleton = this;
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
            _server = new Handlers.Server(this);
            _player = new Handlers.Player(this);
            Exiled.Events.Handlers.Server.RoundStarted += _server.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += _server.OnRoundEnd;
            Exiled.Events.Handlers.Server.WaitingForPlayers += _server.OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.TriggeringTesla += _player.OnTriggerTesla;
        }

        private void UnRegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= _server.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= _server.OnRoundEnd;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= _server.OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.TriggeringTesla -= _player.OnTriggerTesla;

            _server = null;
            _player = null;
        }

        public IEnumerator<float> RunBlackoutTimer()
        {
            yield return Timing.WaitForSeconds(Config.InitialDelay);
            yield return Timing.WaitForSeconds((float)Random.NextDouble() * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
            for (; ; )
            {
                yield return Timing.WaitUntilTrue(() => !(Warhead.IsDetonated || Warhead.IsInProgress));
                Cassie.Message(Config.DoorSentence);

                TimerOn = true;
                yield return Timing.WaitForSeconds(Config.TimeBetweenSentenceAndStart);
                if (Config.Countdown)
                {
                    Cassie.Message("3 . 2 . 1");
                    yield return Timing.WaitForSeconds(3f);
                }

                float blackoutDur = (float)(Random.NextDouble() * (Config.DurationMax - Config.DurationMin) + Config.DurationMin);


                foreach (var door in Map.Doors)
                { 
                    if(door.Type != DoorType.NukeSurface || door.Type != DoorType.Scp914) 
                    {
                        door.IsOpen = false;
                        door.Base.ServerChangeLock(DoorLockReason.AdminCommand, true);
                    }
                }
                yield return Timing.WaitForSeconds(blackoutDur);
                foreach (var door in Map.Doors)
                {
                    door.Base.ServerChangeLock(DoorLockReason.AdminCommand, false);
                }
                Cassie.Message(Config.DoorAfterSentence);
                yield return Timing.WaitForSeconds((float)Random.NextDouble() * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
                TimerOn = false;
            }
        }
    }
}

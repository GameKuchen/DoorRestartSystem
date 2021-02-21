using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;

namespace DoorRestartSystem.Handlers
{
    internal sealed class Server
    {
        private readonly DoorRestartSystem plugin;
        public Server(DoorRestartSystem plugin) => this.plugin = plugin;
        public CoroutineHandle Coroutine;

        public void OnRoundStarted()
        {
            Timing.KillCoroutines(Coroutine);

            int y = plugin.Gen.Next(100);
            if (y < plugin.Config.Spawnchance)
            {
                Coroutine = Timing.RunCoroutine(plugin.RunBlackoutTimer());
            } 
        }

        public void OnRoundEnd(RoundEndedEventArgs ev)
        {
            Timing.KillCoroutines(Coroutine);
        }

        public void OnWaitingForPlayers()
        {
            Timing.KillCoroutines(Coroutine);
        }   
    }
}

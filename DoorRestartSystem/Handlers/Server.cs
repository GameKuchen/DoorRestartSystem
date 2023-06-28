using Exiled.Events.EventArgs.Server;
using MEC;

namespace DoorRestartSystem.Handlers
{
    internal sealed class Server
    {
        private readonly DoorRestartSystemNew _plugin;
        public Server(DoorRestartSystemNew plugin) => _plugin = plugin;
        public CoroutineHandle Coroutine;

        public void OnRoundStarted()
        {
            Timing.KillCoroutines(Coroutine);
            
            if (UnityEngine.Random.Range(0, 100) < _plugin.Config.Spawnchance)
                Coroutine = Timing.RunCoroutine(_plugin.RunBlackoutTimer());
        }
        

        public void OnWaitingForPlayers()
            => Timing.KillCoroutines(Coroutine);

        public void OnRoundEnded(RoundEndedEventArgs ev)
            => Timing.KillCoroutines(Coroutine);
    }
}

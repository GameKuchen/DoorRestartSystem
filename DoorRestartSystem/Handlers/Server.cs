using Exiled.Events.EventArgs;
using MEC;

namespace DoorRestartSystem.Handlers
{
    internal sealed class Server
    {
        private readonly DoorRestartSystem _plugin;
        public Server(DoorRestartSystem plugin) => _plugin = plugin;
        public CoroutineHandle Coroutine;

        public void OnRoundStarted()
        {
            Timing.KillCoroutines(Coroutine);

            var y = _plugin.Gen.Next(100);
            if (y < _plugin.Config.Spawnchance)
            {
                Coroutine = Timing.RunCoroutine(_plugin.RunBlackoutTimer());
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

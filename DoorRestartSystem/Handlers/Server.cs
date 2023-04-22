using Exiled.Events.EventArgs.Server;
using MEC;


namespace DoorRestartSystem2.Handlers
{
    internal sealed class Server
    {
        private readonly DoorRestartSystemNew _plugin;
        public Server(DoorRestartSystemNew plugin) => _plugin = plugin;
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

        public void OnRoundEnding(EndingRoundEventArgs ev)
        {
            Timing.KillCoroutines(Coroutine);
        }

        public void OnWaitingForPlayers()
        {
            Timing.KillCoroutines(Coroutine);
        }   
    }
}

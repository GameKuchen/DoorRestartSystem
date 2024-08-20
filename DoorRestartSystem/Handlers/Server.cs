namespace DoorRestartSystem.Handlers
{

    using System.Collections.Generic;
    using Exiled.Events.EventArgs.Server;
    using MEC;

    internal sealed class Server
    {
        private readonly Plugin _plugin;
        public Server(Plugin plugin) => _plugin = plugin;

        public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

        public void OnRoundStarted()
        {
            if (UnityEngine.Random.Range(0, 100) < _plugin.Config.Spawnchance)
            {
                _plugin.Methods.Init();
                Coroutines.Add(Timing.RunCoroutine(_plugin.Methods.StartLockdownRoutine()));
                
            }
        }


        public void OnWaitingForPlayers()
        {
            _plugin.Methods.Clean();
            foreach (CoroutineHandle handle in Coroutines) Timing.KillCoroutines(handle);
            Coroutines.Clear();

        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            _plugin.Methods.Clean();
            foreach (CoroutineHandle handle in Coroutines) Timing.KillCoroutines(handle);
            Coroutines.Clear();
        }
    }
}

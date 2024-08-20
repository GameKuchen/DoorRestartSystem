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
                
                Coroutines.Add(Timing.RunCoroutine(_plugin.Methods.StartLockdownRoutine()));
                
            }
        }


        public void OnWaitingForPlayers()
        {
            foreach (CoroutineHandle handle in Coroutines) Timing.KillCoroutines(handle);
            foreach (Exiled.API.Features.Room r in Exiled.API.Features.Room.List) r.ResetColor();
            Coroutines.Clear();

        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (CoroutineHandle handle in Coroutines) Timing.KillCoroutines(handle);
            foreach (Exiled.API.Features.Room r in Exiled.API.Features.Room.List) r.ResetColor();
            Coroutines.Clear();
        }
    }
}

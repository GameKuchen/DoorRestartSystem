using Exiled.Events.EventArgs.Server;
using MEC;
using System.Collections.Generic;

namespace DoorRestartSystem.Handlers
{
    internal sealed class Server
    {
        private readonly DoorRestartSystemNew _plugin;
        public Server(DoorRestartSystemNew plugin) => _plugin = plugin;
        public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

        public void OnRoundStarted()
        {
            if (UnityEngine.Random.Range(0, 100) < _plugin.Config.Spawnchance)
            {
                Coroutines.Add(Timing.RunCoroutine(_plugin.RunLockdownTimer()));
                if (_plugin.Config.Flicker) Coroutines.Add(Timing.RunCoroutine(_plugin.FlickeringLights(_plugin.Config.FlickerFrequency)));
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

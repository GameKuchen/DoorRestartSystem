using System;
using System.Collections.Generic;
using EXILED;
using MEC;
using Mirror;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorRestartSystem
{
    public class EventHandlers
    {
        public Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        public bool TeslasDisabled = false;
        public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

        public void OnRoundStart()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);
            Coroutines.Add(Timing.RunCoroutine(plugin.RunBlackoutTimer()));
            TeslasDisabled = false;
        }

        public void OnRoundEnd()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);
        }

        public void OnWaitingForPlayers()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);
            TeslasDisabled = false;
        }

        public void OnTriggerTesla(ref TriggerTeslaEvent ev)
        {
            if (TeslasDisabled)
                ev.Triggerable = false;
        }
    }
}
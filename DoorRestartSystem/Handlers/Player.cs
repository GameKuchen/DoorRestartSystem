using Exiled.Events.EventArgs;

namespace DoorRestartSystem.Handlers
{
    internal class Player
    {
        private readonly DoorRestartSystem _plugin;
        public Player(DoorRestartSystem plugin) => _plugin = plugin;
        private bool TeslasDisabled = false;

        public void OnTriggerTesla(TriggeringTeslaEventArgs ev)
        {
            if (TeslasDisabled)
                ev.IsTriggerable = false;
        }
    }
}
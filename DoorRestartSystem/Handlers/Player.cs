using Exiled.Events.EventArgs;

namespace DoorRestartSystem.Handlers
{
    class Player
    {
        private readonly Doorestartsystem plugin;
        public Player(Doorestartsystem plugin) => this.plugin = plugin;

        

        public bool TeslasDisabled = false;
        public void OnTriggerTesla(TriggeringTeslaEventArgs ev)
        {
            if (TeslasDisabled)
                ev.IsTriggerable = false;
        }
    }
}

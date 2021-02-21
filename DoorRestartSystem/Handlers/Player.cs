using Exiled.Events.EventArgs;

namespace DoorRestartSystem.Handlers
{
    class Player
    {
        private readonly DoorRestartSystem plugin;
        public Player(DoorRestartSystem plugin) => this.plugin = plugin;
        public bool TeslasDisabled = false;

        public void OnTriggerTesla(TriggeringTeslaEventArgs ev)
        {
            if (TeslasDisabled)
                ev.IsTriggerable = false;
        }

        public void onWarheadStart(StartingEventArgs ev)
        {
            DoorRestartSystem.softlockDoors();
        }
    }
}

namespace DoorRestartSystem
{
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.Loader;
    using MEC;
    using UnityEngine;

    public class Methods
    {
        private readonly Plugin _plugin;
        public Methods(Plugin plugin) => _plugin = plugin;

        private readonly List<Room> changedRooms = new List<Room>();
        private readonly List<DoorType> doorTypesToSkip = new List<DoorType>();
        private readonly HashSet<ZoneType> triggeredZones = new HashSet<ZoneType>();
        public void Init()
        {
            // Always skip these doors
            doorTypesToSkip.Add(DoorType.Scp914Door);

            // Nuke doors
            if (_plugin.Config.SkipNukeDoors)
            {
                doorTypesToSkip.Add(DoorType.NukeSurface);
                doorTypesToSkip.Add(DoorType.ElevatorNuke);
            }

            // Unknown doors
            if (_plugin.Config.SkipUnknownDoors)
            {
                doorTypesToSkip.Add(DoorType.UnknownDoor);
                doorTypesToSkip.Add(DoorType.UnknownElevator);
            }

            // Elevators
            if (_plugin.Config.SkipElevators)
            {
                doorTypesToSkip.Add(DoorType.UnknownElevator);
                doorTypesToSkip.Add(DoorType.ElevatorGateA);
                doorTypesToSkip.Add(DoorType.ElevatorGateB);
                doorTypesToSkip.Add(DoorType.ElevatorLczA);
                doorTypesToSkip.Add(DoorType.ElevatorLczB);
                doorTypesToSkip.Add(DoorType.ElevatorNuke);
                doorTypesToSkip.Add(DoorType.ElevatorScp049);
            }

            // Airlocks
            if (_plugin.Config.SkipAirlocks)
            {
                doorTypesToSkip.Add(DoorType.Airlock);
            }

            // SCP Rooms
            if (_plugin.Config.SkipSCPRooms)
            {

                doorTypesToSkip.Add(DoorType.Scp079First);
                doorTypesToSkip.Add(DoorType.Scp079Second);
                doorTypesToSkip.Add(DoorType.Scp049Gate);
                doorTypesToSkip.Add(DoorType.ElevatorScp049);
                doorTypesToSkip.Add(DoorType.Scp096);
                doorTypesToSkip.Add(DoorType.Scp106Primary);
                doorTypesToSkip.Add(DoorType.Scp106Secondary);
                doorTypesToSkip.Add(DoorType.Scp173Bottom);
                doorTypesToSkip.Add(DoorType.Scp173Gate);
                doorTypesToSkip.Add(DoorType.Scp173NewGate);
                doorTypesToSkip.Add(DoorType.Scp330);
                doorTypesToSkip.Add(DoorType.Scp330Chamber);
                doorTypesToSkip.Add(DoorType.Scp914Gate);
                doorTypesToSkip.Add(DoorType.Scp914Door);
                doorTypesToSkip.Add(DoorType.Scp939Cryo);      
            }

            // Armories
            if (_plugin.Config.SkipArmory)
            {
                doorTypesToSkip.Add(DoorType.CheckpointArmoryA);
                doorTypesToSkip.Add(DoorType.CheckpointArmoryB);
                doorTypesToSkip.Add(DoorType.HczArmory);
                doorTypesToSkip.Add(DoorType.LczArmory);
                doorTypesToSkip.Add(DoorType.NukeArmory);
                doorTypesToSkip.Add(DoorType.Scp049Armory);
                doorTypesToSkip.Add(DoorType.Scp079Armory);
                doorTypesToSkip.Add(DoorType.Scp173Armory);
            }

            // Checkpoints
            if (_plugin.Config.SkipCheckpoints)
            {
                doorTypesToSkip.Add(DoorType.CheckpointLczA);
                doorTypesToSkip.Add(DoorType.CheckpointLczB);
                doorTypesToSkip.Add(DoorType.CheckpointEzHczB);
                doorTypesToSkip.Add(DoorType.CheckpointEzHczA);
            }

            // Gate
            if (_plugin.Config.skipCheckpointsGate)
            {
                doorTypesToSkip.Add(DoorType.CheckpointGate);
            }
        }

        public void Clean()
        {
            ResetRoomColors();
            changedRooms.Clear();
            doorTypesToSkip.Clear();
            triggeredZones.Clear();
        }

        public IEnumerator<float> StartLockdownRoutine()
        {

            yield return Timing.WaitForSeconds(_plugin.Config.InitialDelay);
            for (; ; )
            {
                yield return Timing.WaitForSeconds(Loader.Random.Next(_plugin.Config.DelayMin, _plugin.Config.DelayMax));
                yield return Timing.WaitUntilTrue(() => !(Warhead.IsDetonated || Warhead.IsInProgress));

                Cassie.Clear();
                if (_plugin.Config.IsCountdownEnabled)
                {
                    SendDoorRestartSystemCassieMessage(_plugin.Config.CassieMessageCountdown, true);
                    yield return Timing.WaitForSeconds(_plugin.Config.TimeBetweenSentenceAndStart);
                }

                float lockdownDuration = GetLockdownDuration();
                _plugin.Server.Coroutines.Add(Timing.RunCoroutine(HandleLockdownOutcome(lockdownDuration)));
                if (_plugin.Config.Flicker)
                {
                    float flickerDuration = lockdownDuration / _plugin.Config.FlickerFrequency;
                    _plugin.Server.Coroutines.Add(Timing.RunCoroutine(FlickeringLights(flickerDuration, lockdownDuration)));
                }

            }
        }

        private IEnumerator<float> HandleLockdownOutcome(float lockdownDuration)
        {
            ApplyRoomLockdowns(lockdownDuration);
            if (changedRooms.Count > 0)
            {
                SendDoorRestartSystemCassieMessage(_plugin.Config.CassieMessageStart);
                yield return Timing.WaitForSeconds(lockdownDuration);
                SendDoorRestartSystemCassieMessage(_plugin.Config.CassieMessageEnd);
                ResetRoomColors();
                yield return Timing.WaitForSeconds(8.0f);
            }
            else if (_plugin.Config.EnableFacilityLockdown)
            {
                SendDoorRestartSystemCassieMessage(_plugin.Config.CassieMessageStart);
                ApplyFacilityWideLockdown(lockdownDuration);
                yield return Timing.WaitForSeconds(lockdownDuration);
                SendDoorRestartSystemCassieMessage(_plugin.Config.CassieMessageEnd);
                ResetRoomColors();
                yield return Timing.WaitForSeconds(8.0f);
            }
            else
            {
                SendDoorRestartSystemCassieMessage(_plugin.Config.CassieMessageWrong);
            }

            changedRooms.Clear();
            triggeredZones.Clear();
        }
        private IEnumerator<float> FlickeringLights(float flickerDuration, float totalDuration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < totalDuration)
            {
                yield return Timing.WaitForSeconds(flickerDuration / 3);

                foreach (Room room in changedRooms)
                {
                    if (!room.AreLightsOff)
                    {
                        room.TurnOffLights(flickerDuration / 3);
                    }
                }

                yield return Timing.WaitForSeconds(flickerDuration / 3);

                elapsedTime += flickerDuration;
            }
        }

        private void ApplyRoomLockdowns(float lockdownDuration)
        {
            bool isLockdownPerRoom = IsLockdownPerRoom();

            if (isLockdownPerRoom)
            {
                foreach (Room room in Room.List)
                {
                    if (ApplyLockdownToRoom(room, lockdownDuration, IsTriggered(_plugin.Config.ChanceHeavy), IsTriggered(_plugin.Config.ChanceLight), IsTriggered(_plugin.Config.ChanceEntrance), IsTriggered(_plugin.Config.ChanceSurface), IsTriggered(_plugin.Config.ChanceOther), isLockdownPerRoom))
                    {
                        changedRooms.Add(room);
                    }
                }
            }
            else
            {
                bool isHeavy = IsTriggered(_plugin.Config.ChanceHeavy);
                bool isLight = IsTriggered(_plugin.Config.ChanceLight);
                bool isEntrance = IsTriggered(_plugin.Config.ChanceEntrance);
                bool isSurface = IsTriggered(_plugin.Config.ChanceSurface);
                bool isOther = IsTriggered(_plugin.Config.ChanceOther);

                foreach (Room room in Room.List)
                {
                    if (ApplyLockdownToRoom(room, lockdownDuration, isHeavy, isLight, isEntrance, isSurface, isOther, isLockdownPerRoom))
                    {
                        changedRooms.Add(room);
                    }
                }
            }

        }

        private bool ApplyLockdownToRoom(Room room, float lockdownDuration, bool isHeavy, bool isLight, bool isEntrance, bool isSurface, bool isOther, bool isLockdownPerRoom)
        {
            string cassieMessage = string.Empty;
            bool shouldLockdown = false;
            switch (room.Zone)
            {
                case ZoneType zone when zone.Equals(ZoneType.HeavyContainment) && isHeavy:
                    shouldLockdown = true;
                    if (!triggeredZones.Contains(ZoneType.HeavyContainment))
                    {
                        cassieMessage = _plugin.Config.CassieMessageHeavy;
                        triggeredZones.Add(ZoneType.HeavyContainment);
                    }
                    break;

                case ZoneType zone when zone.Equals(ZoneType.LightContainment) && isLight:
                    shouldLockdown = true;
                    if (!triggeredZones.Contains(ZoneType.LightContainment))
                    {
                        cassieMessage = _plugin.Config.CassieMessageLight;
                        triggeredZones.Add(ZoneType.LightContainment);
                    }
                    break;

                case ZoneType zone when zone.Equals(ZoneType.Entrance) && isEntrance:
                    shouldLockdown = true;
                    if (!triggeredZones.Contains(ZoneType.Entrance))
                    {
                        cassieMessage = _plugin.Config.CassieMessageEntrance;
                        triggeredZones.Add(ZoneType.Entrance);
                    }
                    break;

                case ZoneType zone when zone.Equals(ZoneType.Surface) && isSurface:
                    shouldLockdown = true;
                    if (!triggeredZones.Contains(ZoneType.Surface))
                    {
                        cassieMessage = _plugin.Config.CassieMessageSurface;
                        triggeredZones.Add(ZoneType.Surface);
                    }
                    break;

                case ZoneType zone when (zone.Equals(ZoneType.Other) || zone.Equals(ZoneType.Unspecified)) && isOther:
                    shouldLockdown = true;
                    if (!triggeredZones.Contains(ZoneType.Other))
                    {
                        cassieMessage = _plugin.Config.CassieMessageOther;
                        triggeredZones.Add(ZoneType.Other);
                    }
                    break;

                default:
                    break;
            }

            if (shouldLockdown)
            {
                LockdownRoom(room, lockdownDuration, cassieMessage);
                return true;
            }

            return false;
        }

        private bool IsLockdownPerRoom()
        {
            return _plugin.Config.UsePerRoomChances;
        }

        private void LockdownRoom(Room room, float duration, string cassieMessage = "")
        {
            room.Color = new Color(_plugin.Config.LightsColorR, _plugin.Config.LightsColorG, _plugin.Config.LightsColorB);
            foreach (Door door in room.Doors)
            {
                if (!doorTypesToSkip.Contains(door.Type))
                {
                    if (_plugin.Config.CloseDoors)
                    {
                        door.IsOpen = false;
                        door.PlaySound(DoorBeepType.PermissionDenied);
                    }
                    if (!door.IsLocked)
                    {
                        door.Lock(duration, DoorLockType.Isolation);
                        door.PlaySound(DoorBeepType.LockBypassDenied);
                    }
                }
            }

            SendDoorRestartSystemCassieMessage(cassieMessage);
        }

        private bool IsTriggered(float chance)
        {
            return (Loader.Random.NextDouble() * 100) < chance;
        }

        private void SendDoorRestartSystemCassieMessage(string cassieMessage, bool isGlitchy = false)
        {
            if (!(cassieMessage.Length > 0)) return;
            if (isGlitchy)
            {
                Cassie.GlitchyMessage(cassieMessage, _plugin.Config.GlitchChance / 100f, _plugin.Config.JamChance / 100f);
            }
            else
            {
                Cassie.Message(cassieMessage, false, false, false);
            }
        }

        private float GetLockdownDuration()
        {
            return (float)Loader.Random.NextDouble() * (_plugin.Config.DurationMax - _plugin.Config.DurationMin) + _plugin.Config.DurationMin;
        }

        private void ApplyFacilityWideLockdown(float duration)
        {
            foreach (Room room in Room.List)
            {
                LockdownRoom(room, duration, _plugin.Config.CassieMessageFacility);
            }
        }

        private void ResetRoomColors()
        {
            foreach (Room room in changedRooms)
            {
                room.ResetColor();
            }
        }




    }

}

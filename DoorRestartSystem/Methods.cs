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

            // Checkpoints
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
        }

        public void Clean()
        {
            changedRooms.Clear();
            doorTypesToSkip.Clear();
        }

        public IEnumerator<float> StartLockdownRoutine()
        {
            for (; ; )
            {
                yield return Timing.WaitUntilTrue(() => !(Warhead.IsDetonated || Warhead.IsInProgress));

                TriggerInitialCassieMessage();
                yield return Timing.WaitForSeconds(_plugin.Config.TimeBetweenSentenceAndStart);

                float lockdownDuration = GetLockdownDuration();
                ApplyRoomLockdowns(lockdownDuration);

                if (_plugin.Config.Flicker)
                {
                    float flickerDuration = lockdownDuration / _plugin.Config.FlickerFrequency;
                    _plugin.Server.Coroutines.Add(Timing.RunCoroutine(FlickeringLights(flickerDuration, lockdownDuration)));
                }
                _plugin.Server.Coroutines.Add(Timing.RunCoroutine(HandleLockdownOutcome(lockdownDuration)));


                yield return Timing.WaitForSeconds(Loader.Random.Next(_plugin.Config.DelayMin, _plugin.Config.DelayMax));
            }
        }

        private void TriggerInitialCassieMessage()
        {
            Cassie.GlitchyMessage(_plugin.Config.CassieMessageStart, _plugin.Config.GlitchChance / 100f, _plugin.Config.JamChance / 100f);
        }

        private float GetLockdownDuration()
        {
            return (float)Loader.Random.NextDouble() * (_plugin.Config.DurationMax - _plugin.Config.DurationMin) + _plugin.Config.DurationMin;
        }

        private void ApplyRoomLockdowns(float lockdownDuration)
        {
            bool isHeavy = IsRoomTypeTriggered(_plugin.Config.ChanceHeavy, _plugin.Config.UsePerRoomChances);
            bool isLight = IsRoomTypeTriggered(_plugin.Config.ChanceLight, _plugin.Config.UsePerRoomChances);
            bool isEntrance = IsRoomTypeTriggered(_plugin.Config.ChanceEntrance, _plugin.Config.UsePerRoomChances);
            bool isSurface = IsRoomTypeTriggered(_plugin.Config.ChanceSurface, _plugin.Config.UsePerRoomChances);
            bool isOther = IsRoomTypeTriggered(_plugin.Config.ChanceOther, _plugin.Config.UsePerRoomChances);

            if (_plugin.Config.skipCheckpointsGate)
            {
                doorTypesToSkip.Add(DoorType.CheckpointGate);
            }

            foreach (Room room in Room.List)
            {
                if (ApplyLockdownToRoom(room, lockdownDuration, isHeavy, isLight, isEntrance, isSurface, isOther))
                {
                    changedRooms.Add(room);
                }
            }
        }

        private bool ApplyLockdownToRoom(Room room, float lockdownDuration, bool isHeavy, bool isLight, bool isEntrance, bool isSurface, bool isOther)
        {
            if (IsRoomInZone(room, "Hcz", isHeavy, _plugin.Config.CassieMessageHeavy)) return true;
            if (IsRoomInZone(room, "Lcz", isLight, _plugin.Config.CassieMessageLight)) return true;
            if (IsRoomInZone(room, "Ez", isEntrance, _plugin.Config.CassieMessageEntrance)) return true;
            if (IsRoomInZone(room, "Surface", isSurface, _plugin.Config.CassieMessageSurface)) return true;

            if ((!_plugin.Config.UsePerRoomChances && isOther) || (_plugin.Config.UsePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < _plugin.Config.ChanceOther))
            {
                LockdownRoom(room, lockdownDuration, _plugin.Config.CassieMessageOther);
                return true;
            }

            return false;
        }

        private bool IsRoomInZone(Room room, string zoneType, bool isZoneTriggered, string cassieMessage)
        {
            if (room.Type.ToString().Contains(zoneType))
            {
                if (isZoneTriggered)
                {
                    LockdownRoom(room, GetLockdownDuration(), cassieMessage);
                    return true;
                }
            }

            return false;
        }

        private void LockdownRoom(Room room, float duration, string cassieMessage)
        {
            room.Color = new UnityEngine.Color(_plugin.Config.LightsColorR, _plugin.Config.LightsColorG, _plugin.Config.LightsColorB);
            foreach (Door door in room.Doors)
            {
                if (!doorTypesToSkip.Contains(door.Type))
                {
                    if (_plugin.Config.CloseDoors) door.IsOpen = false;
                    if (!door.IsLocked) door.Lock(duration, DoorLockType.Isolation);
                }
            }

            Cassie.Message(cassieMessage, false, false);
        }

        private bool IsRoomTypeTriggered(float chance, bool usePerRoomChances)
        {
            return (!usePerRoomChances && (Loader.Random.NextDouble() * 100) < chance) || (usePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < chance);
        }

        private IEnumerator<float> HandleLockdownOutcome(float lockdownDuration)
        {
            if (changedRooms.Count > 0)
            {
                yield return Timing.WaitForSeconds(lockdownDuration);
                Cassie.Message(_plugin.Config.CassieMessageEnd, false, false);
                ResetRoomColors();
                yield return Timing.WaitForSeconds(8.0f);
            }
            else if (_plugin.Config.EnableFacilityLockdown)
            {
                ApplyFacilityWideLockdown(lockdownDuration);
                yield return Timing.WaitForSeconds(lockdownDuration);
                Cassie.Message(_plugin.Config.CassieMessageEnd, false, false);
                ResetRoomColors();
                yield return Timing.WaitForSeconds(8.0f);
            }
            else
            {
                Cassie.Message(_plugin.Config.CassieMessageWrong, false, false);
            }

            changedRooms.Clear();
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

        private IEnumerator<float> FlickeringLights(float flickerDuration, float totalDuration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < totalDuration)
            {
                foreach (Room room in changedRooms)
                {
                    if (!room.AreLightsOff)
                    {
                        room.TurnOffLights(flickerDuration / 2);
                    }
                }

                yield return Timing.WaitForSeconds(flickerDuration / 2);

                elapsedTime += flickerDuration;
            }

            _plugin.Server.Coroutines.Add(Timing.RunCoroutine(HandleLockdownOutcome(totalDuration)));
        }


    }

}

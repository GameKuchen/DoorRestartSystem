using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Loader;
using MEC;
using Server = DoorRestartSystem.Handlers.Server;

namespace DoorRestartSystem
{ 

    public class DoorRestartSystemNew : Plugin<Config>
    {
        public override string Author => "GameKuchen & iomatix";
        public override string Name => "DoorRestartSystem";
        public override string Prefix => "DRS";
        public override Version Version => new Version(4, 1, 0);
        public override Version RequiredExiledVersion => new Version(7, 0, 0);
        private Server _server;
        public override PluginPriority Priority => PluginPriority.Medium;

        public List<Room> affectedRooms;
        public override void OnEnabled()
        {
            RegisterEvents();
            affectedRooms = new List<Room>();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            affectedRooms.Clear();
            UnRegisterEvents();
            base.OnDisabled();
        }


        private void RegisterEvents()
        {
            _server = new Server(this);
            Exiled.Events.Handlers.Server.RoundStarted += _server.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += _server.OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers += _server.OnWaitingForPlayers;
        }

        private void UnRegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= _server.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= _server.OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= _server.OnWaitingForPlayers;

            _server = null;
        }

        public IEnumerator<float> RunLockdownTimer()
        {

            yield return Timing.WaitForSeconds(Config.InitialDelay);
            yield return Timing.WaitForSeconds(Loader.Random.Next(Config.DelayMin, Config.DelayMax));
            for (;;)
            {
                yield return Timing.WaitUntilTrue(() => !(Warhead.IsDetonated || Warhead.IsInProgress));

                bool isLockdown = false;
                Cassie.GlitchyMessage(Config.CassieMessageStart, Config.GlitchChance / 100, Config.JamChance / 100);
                yield return Timing.WaitForSeconds(Config.TimeBetweenSentenceAndStart);
                float lockdownDur = (float)Loader.Random.NextDouble() * (Config.DurationMax - Config.DurationMin) +
                                    Config.DurationMin;

                Cassie.GlitchyMessage(Config.CassiePostMessage, Config.GlitchChance / 100, Config.JamChance / 100);

                List<ZoneType> affectedZones = new List<ZoneType>();



                switch (Config.UsePerRoomChances)
                {
                    case true:
                        foreach (Room room in Room.List)
                        {
                            if (room.Type.ToString().StartsWith("Hcz"))
                            {
                                if (!(Loader.Random.NextDouble() * 100 < Config.ChanceHeavy)) continue;
                                affectedRooms.Add(room);
                                affectedZones.Add(ZoneType.HeavyContainment);
                                Cassie.Message(Config.CassieMessageHeavy, false, false);
                            }
                            else if (room.Type.ToString().StartsWith("Lcz"))
                            {
                                if (!(Loader.Random.NextDouble() * 100 < Config.ChanceLight)) continue;
                                room.Color = new UnityEngine.Color(Config.LightsColorR, Config.LightsColorG,
                                    Config.LightsColorB);
                                affectedRooms.Add(room);
                                affectedZones.Add(ZoneType.LightContainment);
                                Cassie.Message(Config.CassieMessageHeavy, false, false);
                            }
                            else if (room.Type.ToString().StartsWith("Ez"))
                            {
                                if (!(Loader.Random.NextDouble() * 100 < Config.ChanceLight)) continue;
                                affectedRooms.Add(room);
                                affectedZones.Add(ZoneType.Entrance);
                                Cassie.Message(Config.CassieMessageHeavy, false, false);
                            }
                            else if (room.Type.ToString().StartsWith("Surface"))
                            {
                                if (!(Loader.Random.NextDouble() * 100 < Config.ChanceLight)) continue;
                                affectedRooms.Add(room);
                                affectedZones.Add(ZoneType.Surface);
                                Cassie.Message(Config.CassieMessageHeavy, false, false);
                            }
                            else
                            {
                                if (!(Loader.Random.NextDouble() * 100 < Config.ChanceOther)) continue;
                                affectedRooms.Add(room);
                                affectedZones.Add(ZoneType.Other);
                                Cassie.Message(Config.CassieMessageHeavy, false, false);
                            }
                        }

                        if (affectedRooms.Count != 0)
                        {
                            isLockdown = true;
                            foreach (var room in affectedRooms)
                            {
                                room.Color = new UnityEngine.Color(Config.LightsColorR, Config.LightsColorG,
                                    Config.LightsColorB);
                                foreach (Door d in room.Doors)
                                {
                                    if (d.Type == DoorType.NukeSurface) continue;
                                    if (Config.CloseDoors) d.IsOpen = false;
                                    if (!d.IsLocked) d.Lock(lockdownDur, DoorLockType.Isolation);
                                }
                            }
                        }


                        break;
                    default:
                        //handle other cases
                        break;

                }
                
            if (!isLockdown)
                {
                    if (Config.EnableFacilityLockdown)
                    {
                        foreach (Room r in Room.List)
                        {
                            r.Color = new UnityEngine.Color(Config.LightsColorR, Config.LightsColorG, Config.LightsColorB);
                            affectedRooms.Add(r);
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                if (!d.IsLocked) d.Lock(lockdownDur, DoorLockType.Isolation);
                            }
                        }
                        isLockdown = true;
                        Cassie.Message(Config.CassieMessageFacility, false, false);
                    }
                }
                else if (Config.UsePerRoomChances) Cassie.Message(Config.CassieMessageOther, false, false);

                // End Event
                if (isLockdown)
                {
                    yield return Timing.WaitForSeconds(lockdownDur);
                    Cassie.Message(Config.CassieMessageEnd, false, false);
                    foreach (Room r in affectedRooms) r.ResetColor();
                    yield return Timing.WaitForSeconds(8.0f);

                }
                else Cassie.Message(Config.CassieMessageWrong, false, false);

                affectedRooms.Clear();

                yield return Timing.WaitForSeconds(Loader.Random.Next(Config.DelayMin, Config.DelayMax));

            }
        }
        public IEnumerator<float> FlickeringLights(float dur)
        {
            yield return Timing.WaitForSeconds(Config.InitialDelay);
            yield return Timing.WaitForSeconds(dur);
            for (; ; )
            {
                yield return Timing.WaitForSeconds(dur);
                foreach (Room r in affectedRooms)
                {

                    if (!r.AreLightsOff)
                    {
                        r.TurnOffLights(dur / 2);
                    }
                }
                yield return Timing.WaitForSeconds(dur / 2);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
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
        public override Version Version => new Version(4, 0, 0);
        public override Version RequiredExiledVersion => new Version(7, 0, 0);
        private Server _server;
        public override PluginPriority Priority => PluginPriority.Medium;

        public List<Room> changedRooms;
        public override void OnEnabled()
        {
            RegisterEvents();
            changedRooms = new List<Room>();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            changedRooms.Clear();
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
            for (; ; )
            {
                yield return Timing.WaitUntilTrue(() => !(Warhead.IsDetonated || Warhead.IsInProgress));

                bool isLockdown = false;
                Cassie.GlitchyMessage(Config.CassieMessageStart, Config.GlitchChance / 100, Config.JamChance / 100);
                yield return Timing.WaitForSeconds(Config.TimeBetweenSentenceAndStart);
                float lockdownDur = (float)Loader.Random.NextDouble() * (Config.DurationMax - Config.DurationMin) + Config.DurationMin;

                Cassie.GlitchyMessage(Config.CassiePostMessage, Config.GlitchChance / 100, Config.JamChance / 100);


                List<ZoneType> zones = new List<ZoneType>();

                bool isOtherMessage = false;
                bool isHeavy = false;
                bool isLight = false;
                bool isEnt = false;
                bool isSur = false;
                bool isOth = false;

                bool isHeavyMsg = false;
                bool isLightMsg = false;
                bool isEntMsg = false;
                bool isSurMsg = false;
                bool isOthMsg = false;

                if (((float)Loader.Random.NextDouble() * 100) < Config.ChanceHeavy) isHeavy = true;
                if (((float)Loader.Random.NextDouble() * 100) < Config.ChanceLight) isLight = true;
                if (((float)Loader.Random.NextDouble() * 100) < Config.ChanceEntrance) isEnt = true;
                if (((float)Loader.Random.NextDouble() * 100) < Config.ChanceSurface) isSur = true;
                if (((float)Loader.Random.NextDouble() * 100) < Config.ChanceOther) isOth = true;

                foreach (Room r in Room.List)
                {
                    //Heavy 
                    if (r.Type.ToString().Contains("Hcz"))
                    {
                        if ((!Config.UsePerRoomChances && isHeavy) || (Config.UsePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < Config.ChanceHeavy))
                        {
                            r.Color = new UnityEngine.Color(Config.LightsColorR, Config.LightsColorG, Config.LightsColorB);
                            changedRooms.Add(r);
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
                            }
                            isLockdown = true;
                        }
                        if (!Config.UsePerRoomChances && isHeavy && !isHeavyMsg) { Cassie.Message(Config.CassieMessageHeavy, false, false); isHeavyMsg = true; }
                    }
                    //Light
                    else if (r.Type.ToString().Contains("Lcz"))
                    {
                        if ((!Config.UsePerRoomChances && isLight) || (Config.UsePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < Config.ChanceLight))
                        {
                            r.Color = new UnityEngine.Color(Config.LightsColorR, Config.LightsColorG, Config.LightsColorB);
                            changedRooms.Add(r);
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
                            }
                            isLockdown = true;
                        }
                        if (!Config.UsePerRoomChances && isLight && !isLightMsg) { Cassie.Message(Config.CassieMessageLight, false, false); isLightMsg = true; }
                    }
                    //Entrance 
                    else if (r.Type.ToString().Contains("Ez"))
                    {
                        if ((!Config.UsePerRoomChances && isEnt) || (Config.UsePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < Config.ChanceEntrance))
                        {
                            r.Color = new UnityEngine.Color(Config.LightsColorR, Config.LightsColorG, Config.LightsColorB);
                            changedRooms.Add(r);
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
                            }
                            isLockdown = true;
                        }
                        if (!Config.UsePerRoomChances && isEnt && !isEntMsg) { Cassie.Message(Config.CassieMessageEntrance, false, false); isEntMsg = true; }
                    }
                    //Surface 
                    else if (r.Type.ToString().Contains("Surface"))
                    {
                        if ((!Config.UsePerRoomChances && isSur) || (Config.UsePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < Config.ChanceSurface))
                        {
                            r.Color = new UnityEngine.Color(Config.LightsColorR, Config.LightsColorG, Config.LightsColorB);
                            changedRooms.Add(r);
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
                            }
                            isLockdown = true;
                        }
                        if (!Config.UsePerRoomChances && isSur && !isSurMsg) { Cassie.Message(Config.CassieMessageSurface, false, false); isSurMsg = true; }
                    }
                    //Misc
                    else
                    {
                        if ((!Config.UsePerRoomChances && isOth) || (Config.UsePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < Config.ChanceOther))
                        {
                            r.Color = new UnityEngine.Color(Config.LightsColorR, Config.LightsColorG, Config.LightsColorB);
                            changedRooms.Add(r);
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
                            }
                            isLockdown = true;
                        }
                        if (!Config.UsePerRoomChances && isOth && !isOthMsg) { Cassie.Message(Config.CassieMessageOther, false, false); isOthMsg = true; }
                    }
                }
                if (!isLockdown)
                {
                    if (Config.EnableFacilityLockdown)
                    {
                        foreach (Room r in Room.List)
                        {
                            r.Color = new UnityEngine.Color(Config.LightsColorR, Config.LightsColorG, Config.LightsColorB);
                            changedRooms.Add(r);
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
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
                    foreach (Room r in changedRooms)
                    {

                        foreach (Door d in r.Doors)
                        {
                            if (d.Type == DoorType.NukeSurface) continue;
                            d.ChangeLock(DoorLockType.SpecialDoorFeature);
                        }
                        if (Config.Flicker && r.AreLightsOff) r.TurnOffLights(1f);
                        r.ResetColor();
                    }
                    yield return Timing.WaitForSeconds(8.0f);

                }
                else Cassie.Message(Config.CassieMessageWrong, false, false);


                changedRooms.Clear();
                yield return Timing.WaitForSeconds(Loader.Random.Next(Config.DelayMin, Config.DelayMax));

            }
        }
        public IEnumerator<float> FlickeringLights(float dur)
        {
            yield return Timing.WaitForSeconds(Config.InitialDelay);
            yield return Timing.WaitForSeconds(dur);
            for (; ; )
            {
                Timing.WaitForSeconds(dur);
                foreach (Room r in changedRooms)
                {
                    
                    if (!r.AreLightsOff)
                    {
                        r.TurnOffLights(dur);
                    }
                }
                Timing.WaitForSeconds(dur);
                yield return Timing.WaitForSeconds(dur);
            }
        }
    }
}
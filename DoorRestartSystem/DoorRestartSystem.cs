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

        public override void OnEnabled()
        {
            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines(_server.Coroutine);
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

        public IEnumerator<float> RunBlackoutTimer()
        {

            yield return Timing.WaitForSeconds(Config.InitialDelay);
            yield return Timing.WaitForSeconds(Loader.Random.Next(Config.DelayMin, Config.DelayMax));
            for (; ; )
            {
                yield return Timing.WaitUntilTrue(() => !(Warhead.IsDetonated || Warhead.IsInProgress));

                bool isLockdown = false;
                List<Door> changedDoors = new List<Door>();
                Cassie.Message(Config.CassieMessageStart, false, true);
                yield return Timing.WaitForSeconds(Config.TimeBetweenSentenceAndStart);
                float lockdownDur = (float)Loader.Random.NextDouble() * (Config.DurationMax - Config.DurationMin) + Config.DurationMin;

                Cassie.Message(Config.CassiePostMessage, false, true);


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
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
                                changedDoors.Add(d);
                            }
                            isLockdown = true;
                        }
                        if (!Config.UsePerRoomChances && isHeavy && !isHeavyMsg) { Cassie.Message(Config.CassieMessageHeavy, false, true); isHeavyMsg = true; }
                    }
                    //Light
                    else if (r.Type.ToString().Contains("Lcz"))
                    {
                        if ((!Config.UsePerRoomChances && isLight) || (Config.UsePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < Config.ChanceLight))
                        {
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
                                changedDoors.Add(d);
                            }
                            isLockdown = true;
                        }
                        if (!Config.UsePerRoomChances && isLight && !isLightMsg) { Cassie.Message(Config.CassieMessageLight, false, true); isLightMsg = true; }
                    }
                    //Entrance 
                    else if (r.Type.ToString().Contains("Ez"))
                    {
                        if ((!Config.UsePerRoomChances && isEnt) || (Config.UsePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < Config.ChanceEntrance))
                        {
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
                                changedDoors.Add(d);
                            }
                            isLockdown = true;
                        }
                        if (!Config.UsePerRoomChances && isEnt && !isEntMsg) { Cassie.Message(Config.CassieMessageEntrance, false, true); isEntMsg = true; }
                    }
                    //Surface 
                    else if (r.Type.ToString().Contains("Surface"))
                    {
                        if ((!Config.UsePerRoomChances && isSur) || (Config.UsePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < Config.ChanceSurface))
                        {
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
                                changedDoors.Add(d);
                            }
                            isLockdown = true;
                        }
                        if (!Config.UsePerRoomChances && isSur && !isSurMsg) { Cassie.Message(Config.CassieMessageSurface, false, true); isSurMsg = true; }
                    }
                    //Misc
                    else
                    {
                        if ((!Config.UsePerRoomChances && isOth) || (Config.UsePerRoomChances && ((float)Loader.Random.NextDouble() * 100) < Config.ChanceOther))
                        {
                            foreach (Door d in r.Doors)
                            {
                                if (d.Type == DoorType.NukeSurface) continue;
                                if (Config.CloseDoors) d.IsOpen = false;
                                d.ChangeLock(DoorLockType.SpecialDoorFeature);
                                changedDoors.Add(d);
                            }
                            isLockdown = true;
                        }
                        if (!Config.UsePerRoomChances && isOth && !isOthMsg) { Cassie.Message(Config.CassieMessageOther, false, true); isOthMsg = true; }
                    }
                }
                if (!isLockdown && Config.EnableFacilityLockdown)
                {

                    foreach (Door d in Door.List)
                    {
                        if (d.Type == DoorType.NukeSurface) continue;
                        if (Config.CloseDoors) d.IsOpen = false;
                        d.ChangeLock(DoorLockType.SpecialDoorFeature);
                        changedDoors.Add(d);
                    }
                    isLockdown = true;
                    Cassie.Message(Config.CassieMessageFacility, false, true);
                }



                // End Event
                if (isLockdown)
                {
                    yield return Timing.WaitForSeconds(lockdownDur - Config.TimeBetweenSentenceAndStart);
                    foreach (Door d in changedDoors)
                    {
                        if (d.Type == DoorType.NukeSurface) continue;
                        d.ChangeLock(DoorLockType.SpecialDoorFeature);
                    }
                    Cassie.Message(Config.CassieMessageEnd, false, false);
                    yield return Timing.WaitForSeconds(8.0f);

                }
                else Cassie.Message(Config.CassieMessageWrong, false, false);



                yield return Timing.WaitForSeconds(Loader.Random.Next(Config.DelayMin, Config.DelayMax));

            }
        }
    }
}
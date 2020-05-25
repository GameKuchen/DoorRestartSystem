using System;
using EXILED;
using EXILED.Extensions;
using MEC;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorRestartSystem
{

    public class Plugin : EXILED.Plugin
    {
        public Random Gen = new Random();

        public EventHandlers EventHandlers;
        public MTFRespawn Respawn;
        public static bool TimerOn = true;

        public float InitialDelay;
        public float DurationMin;
        public float DurationMax;
        public int DelayMax;
        public int DelayMin;


        public override void OnEnable()
        {
            try
            {
                Info("loaded.");
                ReloadConfig();
                Info("Configs loaded.");
                EventHandlers = new EventHandlers(this);

                Events.RoundStartEvent += EventHandlers.OnRoundStart;
                Events.RoundEndEvent += EventHandlers.OnRoundEnd;
                Events.WaitingForPlayersEvent += EventHandlers.OnWaitingForPlayers;
                Events.TriggerTeslaEvent += EventHandlers.OnTriggerTesla;
            }
            catch (Exception e)
            {
                Log.Error($ "OnEnable Error: {e}");
            }
        }

        public void ReloadConfig()
        {
            Log.Info($ "Config Path: {Config.Path}");
            InitialDelay = Config.GetFloat("drs_initial_delay", 120f);
            DurationMin = Config.GetFloat("drs_dur_min", 5f);
            DurationMax = Config.GetFloat("drs_dur_max", 10);
            DelayMin = Config.GetInt("drs_delay_min", 180);
            DelayMax = Config.GetInt("drs_delay_max", 300);
        }
        public override void OnDisable()
        {
            foreach (CoroutineHandle handle in EventHandlers.Coroutines)
                Timing.KillCoroutines(handle);
            Events.RoundStartEvent -= EventHandlers.OnRoundStart;
            Events.RoundEndEvent -= EventHandlers.OnRoundEnd;
            Events.WaitingForPlayersEvent -= EventHandlers.OnWaitingForPlayers;
            Events.TriggerTeslaEvent -= EventHandlers.OnTriggerTesla;
            EventHandlers = null;
        }

        public override void OnReload()
        {

        }

        public override string getName
        {
            get;
        } = "DoorRestartSystem";



        public IEnumerator<float> RunBlackoutTimer()
        {
            if (Respawn == null)
                Respawn = PlayerManager.localPlayer.GetComponent<MTFRespawn>();
            yield
            return Timing.WaitForSeconds(InitialDelay);

            for (; ; )
            {
                Respawn.RpcPlayCustomAnnouncement("WARNING . DOOR SOFTWARE REPAIR IN t minus 20 seconds .", false, true);


                TimerOn = true;
                yield
                return Timing.WaitForSeconds(8.7f);
                float blackoutDur = DurationMax;




                foreach (Door door in UnityEngine.Object.FindObjectsOfType<Door>())
                {


                    door.SetStateWithSound(true);
                    door.NetworkisOpen = false;
                    door.Networklocked = true;
                }




                yield
                return Timing.WaitForSeconds(blackoutDur - 8.7f);
                foreach (Door door in UnityEngine.Object.FindObjectsOfType<Door>())
                {
                    door.SetStateWithSound(true);
                    door.Networklocked = false;
                }
                Respawn.RpcPlayCustomAnnouncement("DOOR SOFTWARE REPAIR COMPLETE", false, true);
                yield
                return Timing.WaitForSeconds(8.7f);
                TimerOn = false;

            }
        }
    }

}
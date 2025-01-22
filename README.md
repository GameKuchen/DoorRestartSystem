## Door-Restart-System 4.1
![GitHub release (latest by date)](https://img.shields.io/github/downloads/gamekuchen/DoorRestartSystem/v4.1.0/total?style=for-the-badge)
## Info
This is a <SCP SL Exiled 7.1> plugin that allows for the facility to have a "Door Software Restart". 
The plugin closes and full locks all doors for the configurated time and then unlocks them again.

Rumors say that these "glitches" happen because someone spilled their coffee on the system in a rush to leave the facility.

## Configs:
```DRS:
# Enable or disable DoorRestartSystem.
  is_enabled: true
  # The InitialDelay before the first Door Restart can happen
  initial_delay: 120
  # The Minumum Duration of the Lockdown
  duration_min: 5
  # The Maximum Duration of the Lockdown
  duration_max: 15
  # The The Minumum Delay before the next the Lockdown
  delay_min: 300
  # The The Maxiumum Delay before the next the Lockdown
  delay_max: 500
  # The chance that a Round even has Doorsystemrestarts
  spawnchance: 45
  # The sentence it transmits via Cassie before the System gets restarted
  door_sentence: pitch_0.2 .g4 . .g4 pitch_1 door control system pitch_0.25 .g1 pitch_0.9 malfunction pitch_1 . initializing repair
  # Enable 3 . 2 . 1 announcement
  countdown: false
  # The time between the sentence and the 3 . 2 . 1 announcement
  time_between_sentence_and_start: 12
  # The sentence it transmits via Cassie after the system got restarted
  door_after_sentence: DOOR CONTROL SYSTEM REPAIR COMPLETE```

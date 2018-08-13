## Centralized Climate Control
A [RimWorld](http://store.steampowered.com/app/294100/RimWorld/) game mod for providing a better late game temperature management system.

### About the Mod
This Mod allows centralized air cooling/heating. Build large Climate Systems in your map away from your buildings. This overcomes certain late game drawbacks like building excessive coolers and ramping up the electricity usage as we enter late game.

![Banner](/Misc/Banners/Banner1.jpg)

[More screenshots](https://github.com/vasumahesh1/CentralizedClimateControlMod/tree/Source/Misc/Banners)

This mod works by adding a few buildings namely:
- Air Intake Machine
- Air Climate Unit
- Air Vents

We connect these machines through Pipes. The Mod also manages various Pipe Networks that the user can create. It also maintains air just like electricity by generating it at Intakes and exhausting it at Vents.

The Mod calculates two Efficiencies which are:
- Thermal Efficiency

    Depends on the speed at which the climate units can heat/cool the air blowing in from the intake machines.

- Flow Efficiency

    Depends on the number of vents open in the network and the total intake air of the network.

These two efficiencies ultimately dictate the rate and amount of change in temperature. Certain ingame events like Cold Snap, Heat waves or Solar Flare can affect temperatures and the Air Network's reliability is defined by these two params.

There are some larger intakes and climate units available in the late game for larger maps.

### Suggestion/Issue List
- [ ] Optimize a bit more
- [x] Fix Breakdown Inspect String
- [x] Fix Temperature Change
- [x] Switch to change pipes at Vent
- [x] Wall mounted Vents
- [x] Larger Machines for Big maps

### Working with the Code
The code base is primarily C#. I use mix tasks from Elixir to bundle the mod automatically for NexusMods, Github and Google Drive.

### Downloads
- [Github Releases](https://github.com/vasumahesh1/CentralizedClimateControlMod/releases)
- [Steam Workshop](http://steamcommunity.com/sharedfiles/filedetails/?id=973091113)
- [Google Drive](https://drive.google.com/drive/folders/0B08U3R0FGDNCaWowSC1wNDg1ZW8?usp=sharing)
- [Nexus Mods](http://www.nexusmods.com/rimworld/mods/196/)


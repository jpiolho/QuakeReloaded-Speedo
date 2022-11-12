# Speedo
<p align="center">An engine mod for Quake Enhanced that an in-game velocity meter to the UI</p>
<p align="center">This mod uses <a href="https://github.com/Reloaded-Project/Reloaded-II">Reloaded-II</a> and <a href="https://github.com/jpiolho/QuakeReloaded">QuakeReloaded</a></p>
<p align="center"><img width="256" height="256" alt="Logo" src="https://github.com/jpiolho/QuakeReloaded-Speedo/blob/main/Speedo/Preview.png"></p>


# How to configure
With this mod, the following cvars are available:
* `scr_speed` - 0: Off, 1: Normal, 2: Horizontal speed only
* `scr_speed_x` - Customize the X position of the velocity meter
* `scr_speed_y` - Customize the Y position of the velocity meter
* `scr_speed_style` - 0: Normal, 1: Minimalist
* `scr_speed_update` - How fast the meter should update (in seconds). Bigger numbers means the number is more stable and accurate whereas smaller values are more noisy.

# How to install
1. Download and install [Reloaded-II](https://github.com/Reloaded-Project/Reloaded-II) if you don't have it
2. Install [QuakeReloaded](https://github.com/jpiolho/QuakeReloaded) mod if you don't have it
3. Download the latest version of Speedo at the [releases](https://github.com/jpiolho/QuakeReloaded-Speedo/releases) page

# FAQ
## How does the meter work?
The meter calculates speed by getting the distance between the last frame and the current frame. Then saves the highest speed achieved between UI updates and displays the highest speed achieved during that update cycle.

## Does it work in multiplayer?
Yes

## Does it work with X mod?
Yes
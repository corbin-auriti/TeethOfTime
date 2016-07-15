-=================-
-= Teeth of Time =-
-=================-

Teeth of Time is a 3rd party Minecraft tool made by Commander Keen.
It's designed to simulate time decay and nature reclaiming
on Minecraft worlds. This is done in a very realistic way, making
blocks to fall off or tople and lay on the ground. Nature takes
over human creations by covering them in grass, trees and other
natural features. Everything is fully customisable, configs
can be easily made, stored and chosed at start of the application.

Teeth of Time runs on the C# library Substrate created by Jaquadro.

If you have any suggestions, bug reports or just good old comments,
post it in Teeth of Time forum thread:

http://www.minecraftforum.net/topic/269176-teeth-of-time-ruin-your-world-v031-brawler/


-- FEATURE LIST --

	* Unsupported blocks fall, supported blocks topple
	* Blocks decay into others (cobble into mossy cobble, etc.)
	* Nature takes over human creations, using Minecraft itself to repopulate all chunks
	* A Perlin filter to add non-uniform destruction and more detail
	* Ability to ruin either the whole world or selected chunks only
	* Everything can be customised in config files wich can be easily chosen at start of the app
	* Multiple "official" configs included, with varying degree of decay
	* Relighting
	* Fluids recalculation
	* GUI frontend provided to run the program
	* Batch file compatible
	* Mod compatible
	
	
-- REQUIREMENTS --

	> Windows XP, Vista or 7
	> .NET Framework 4

	
-- HOW TO USE --

- Installation -

To install the application, just extract all files in the .rar file to a directory of your choice.

- Running Teeth of Time -

Simply run the included "TeethFrontEnd.exe" file. You will see a blank screen with the comboboxes below
it and the "Age selected world" button. Pick a world from the left combobox. The application will freeze for
a while. Don't turn it off, it's just loading the map. Wait until the map loads. After that, you will see
your map from a topdown perspective, with a grid symbolizing the chunks. Clicking into the grid will select
the chunk you have clicked, clicking a selected chunk will unselect it. Selected chunks can be recognized by
having yellow borders. If you don't select any chunks, Teeth of Time will run the algorithm on all chunks.
When you are done with selecting chunks, pick a config from the right list and click the "Age selected World".

If you want to run Teeth of Time manually, run "TeethOfTime.exe path/to/world configname" from
the commandline (without the commas of course). This will age all chunks in the selected world.


-- OFFICIAL CONFIGS --

Teeth of Time comes with a selection of official configs. Here is their description:

> default.cfg - Default commented config. Is used in case of user-specified config missing.
> light.cfg - Light decay, useful to add cosmetic look but not hamper movement much.
> moderate.cfg - Strong decay. Most structures still stand but are severely damaged.
> hard_quality.cfg - Total destruction. This config sacrifices speed for quality.
> hard_speed.cfg - Total destruction. This config sacrifices quality for speed.
> wastelands.cfg - Hard_quality config modified to make the terrain a barren wasteland.
> warfare.cfg - Strong damage, signs of running war everywhere.


-- MAKING YOUR OWN CONFIG

You can easily make your config by duplicating the "default.cfg" config file. The default.cfg
is fully commented and is identical to the "moderate.cfg" config file, making it the best
option to start your own configs from. Teeth of Time is fully compatible with mods, you
just have to include the ID values of mod blocks in the arrays of the config file.

Be warned - using blocks that need lots of calculations, such as TNT or cacti, may crash
the game.

Note that all config files have to be in the "configs" directory in order to be recognized
by Teeth of Time.


-- RECOMMENDED TOOLS --

There is a variety of map generating tools usable with Teeth of Time. I recommend these:

Robson_'s Mace city generator - http://www.minecraftforum.net/topic/357201-mace-v150-random-city-generator/
( Suggested config: all work well )

Eggplant!'s McDungeon dungeon generator - http://www.minecraftforum.net/topic/230446-15-17-mcdungeon-v023/
( Suggested config: light.cfg )


-- FAQ --

Q: After using Teeth of Time on my world, it shows me "saving chunks" then a blackscreen appears!
A: Your computer isn't strong enough to handle Minecraft updating all chunks at once. Just exit Minecraft and
run the world repeatedly until no crashing happens.


-- CREDITS --

Teeth of Time would not exist or would be much pain to develop if it weren't of these great people:

Thanks to:
- Jaquadro for his Substrate Map editing API ( http://www.minecraftforum.net/topic/245996-sdk-substrate-map-editing-library-for-cnet-060/ )
- Kuba1920 for his castle world I used to test v0.1 of the tool. ( http://www.minecraftworldmap.com/worlds/2bFTi )
- Oldshoes for Broville I tested v0.2 on. ( http://www.minecraftforum.net/topic/21991-super-neato-creation-broville-v9-v10-in-the-works/ )
- Robson_ for Mace City Generator 1.2.0 I tested beta 0.3 with. ( http://www.minecraftforum.net/topic/357201-mace-v120-random-city-generator/page__st__40 )
- Coau14, Angry Lolrus and Arcade909 for testing the release candidates for 1.0
- Minecraft forum community for keeping interest for this project in me. 


-- LICENSE --

Teeth of Time is licensed under the MIT Permissive license:


Copyright (C) 2011 by Tomas Suchy

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
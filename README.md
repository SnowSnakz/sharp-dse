# sharp-dse
SharpDSE is a C# library that provides tools for loading and editing Procyon Digital Sound Elements (DSE) related files.

It's very heavily based on the research notes compiled by psy_commando, and the people over at Project Pokémon.

Right now the project is still in it's early stages.

## What can I do with it at the moment?
* If you have a Pokémon Mystery Dungeon: Explorers of Sky ROM, you can extra the rom and run bgm.swd through the SharpDSE.Test program to view information on the file and play all of the samples (currently without envolope.)

## Roadmap
### SWDL Files
- [x] Load SWD files and display basic information
- [ ] Load SWD chunks and display basic information
  - [x] WaveInfoChunk (WAVI)
  - [ ] ProgramInfoChunk (PRGI)
  - [ ] KeyGroupChunk (KGRP)
  - [x] PcmDataChunk (PCMD)
  - [x] ~~EndOfDataChunk (EOD\x20)~~ Nothing to load or display.

### SMDL Files
- [ ] Load SMD files and display basic information
- [ ] Ability to render songs as PCM streams (for use in OpenAL, or other audio playing libraries)

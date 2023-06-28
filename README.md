# sharp-dse
SharpDSE is a C# library that provides tools for loading and editing Procyon Digital Sound Elements (DSE) related files.

It's very heavily based on the research notes compiled by psy_commando, and the people over at Project Pokémon.

Right now the project is still in it's early stages.

## Roadmap
### SWDL Files
- [x] Load SWD files and display basic information
- [ ] Load SWD chunks and display basic information
  - [ ] WaveInfoChunk (WAVI)
  - [ ] ProgramInfoChunk (PRGI)
  - [ ] KeyGroupChunk (KGRP)
  - [ ] PcmDataChunk (PCMD)
  - [x] ~~EndOfDataChunk (EOD\x20)~~ Nothing to load or display.

### SMDL Files
- [ ] Load SMD files and display basic information
- [ ] Ability to render songs as PCM streams (for use in OpenAL, or other audio playing libraries)

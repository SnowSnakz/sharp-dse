# sharp-dse
SharpDSE is a C# library that provides tools for loading and editing Procyon Digital Sound Elements (DSE) related files.

It's very heavily based on the research notes compiled by psy_commando, and the people over at Project Pokémon.

Right now the project is still in it's early stages.

## What can I do with it at the moment?
* If you have a Pokémon Mystery Dungeon: Explorers of Sky ROM, you can extract the rom and run bgm.swd through the SharpDSE.Test program to view information on the file and play all of the samples (currently without envolope.)
  This is the output I got:
  ```
Version:       0x0415
File Name:     B.SWD
Creation Date: 11/19/2008 7:45:06 AM
Chunks: 3
  At 0x00000050: Wave Information Chunk
    Label:  wavi {0x77, 0x61, 0x76, 0x69}
    Length: 0x00005570
    Sample Count: 312
  At 0x000055D0: Pcm Data Chunk
    Label:  pcmd {0x70, 0x63, 0x6D, 0x64}
    Length: 0x00177698
  At 0x0017CC80: End of Data
    Label:  eod  {0x65, 0x6F, 0x64, 0x20}
    Length: 0x00000000

Would you like to play all of the samples from the swdl file? [Y/N, default=N]: y
Loading 0 of 312... (4-bits AdPcm - 2232 samples @ 22050hz)
Playing... Done!

Loading 1 of 312... (4-bits AdPcm - 8024 samples @ 22050hz)
Playing... Done!

Loading 2 of 312... (4-bits AdPcm - 4160 samples @ 22050hz)
Playing... Done!

[etc...]
```

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

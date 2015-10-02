# CueToOgg

ORIGINS

This program originates at the GOG Quake forum... so that's it's home:

https://www.gog.com/forum/quake_series/my_oneclick_audioextractor

DOWNLOAD

https://github.com/hansschmucker/CueToOgg/tree/master/Binaries

LICENSE

GNU LESSER GENERAL PUBLIC LICENSE 2.1

SOURCE

https://github.com/hansschmucker/CueToOgg

VIDEO

http://youtu.be/-Arx7QFo0I4

(Also shows you where to move the files so that QuakeSpasm and Darkplaces will recognize them)


In case anybody missed it: Quake on GOG contains the CD images with the full soundtrack, but most engines can't use them ... and GOG can't provide pre-converted files, because of an old licensing issue.


Converting the files yourself isn't overly complicated, but it means hunting down some software... and that means that fewer people will play Quake, because that's too much hassle. And that's something I can't allow.


So, I've written a very simple cue sheet parser, which extracts the audio, then hands it over to FFMPEG (which is included in the download and doesn't need any form of installation either) to generate 256kbit OGG files.
It is NOT a special GOG-Quake-Audio converter... which would have been a lot simpler, but would probably cause licensing issues as well... it really works on most CUE/BIN files that don't already use compressed audio, for example Alone in the Dark 1/2/3, Descent 2, Earthworm Jim 1/2, Quake & Mission Packs 1/2, Rayman Forever, Redneck Rampage & Rides Again, Settlers 2 Gold and Shattered Steel.


Just put it in the same directory as your CUE files, in this case the Quake directory (or anywhere really... if it can't find any CUE files in the current directory, it will just ask for the path) and run it. Done.


Only thing it can't do (because that would be Quake-specific) is put the music in the right place, that's something you'll have to do yourself. It will create a directory corresponding to the BIN file and put the OGG files (which, lucky us, use the same naming conventions as most Quake engines, so you won't even have to rename them) in there.


For Quake that means you'll end up with three directories
game, gamea, gamed
containing track02.ogg, track03.ogg and so on.


GAME is for vanilla quake.
Create a directory named MUSIC in the ID1 folder and move these files in there.


GAMEA is for Mission Pack 1 - Scourge of Armagon
Create a directory named MUSIC in the HIPNOTIC folder and move these files in there.


GAMED Mission Pack 2 - Dissolution of Eternity
Create a directory named MUSIC in the ROGUE folder and move these files in there.


That's all there is to do.


For the audiophiles: No, it doesn't de-emphasize the sound yet. I doubt that most people will notice and in any case it's better than no music at all. 

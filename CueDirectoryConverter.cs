using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CueToOgg
{
    public class CueDirectoryConverter
    {

        public List<Track> tracks;

        public LogApplication app;

        public CueDirectoryConverter(LogApplication aApp)
        {
            app = aApp;
        }

        public void StartProcessing()
        {

            if (!File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\bin\\ffmpeg.exe"))
            {
                app.FatalExit("FFMPEG not found. Cannot continue.\n");
                return;
            }

            string[] files;
            files =
                Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath), "*.cue").Concat(
                Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath), "*.inst")).ToArray()
                ;

            if (files == null || files.Length == 0)
            {

                app.Log("No cue sheet found in current directory. Please select the directory containing them.\n");

                var pathFinder = new FolderBrowserDialog();
                var result = pathFinder.ShowDialog();
                if (result != DialogResult.OK || pathFinder.SelectedPath == null || pathFinder.SelectedPath == "" || !Directory.Exists(pathFinder.SelectedPath))
                {
                    app.FatalExit("No path selected. Cannot continue.\n");
                    return;
                }

                files =
                    Directory.GetFiles(pathFinder.SelectedPath, "*.cue").Concat(
                    Directory.GetFiles(pathFinder.SelectedPath, "*.inst")).ToArray()
                    ;

                if (files.Length == 0)
                {
                    app.FatalExit("No cue sheet found in path. Cannot continue.\n");
                    return;
                }
            }

            app.Log(files.Length.ToString() + " cue sheets found in path.\n");

            for (var i = 0; i < files.Length; i++)
            {
                tracks = new List<Track>();
                SplitCue(files[i]);
                CalcLengthAndByteOffsets();
                ProcessSegments();
            }
            app.Log("Done\n");
            app.Alert("Done");
            app.Exit();
        }


        private void CalcLengthAndByteOffsets()
        {
            var runningOffset = 0;
            for (var i = 0; i < (tracks.Count - 1); i++)
            {
                var nextOffset = tracks[i + 1].offsetInSectors;
                var currentOffset = tracks[i].offsetInSectors;
                var lengthInSectors = nextOffset - currentOffset - tracks[i].preGap;
                tracks[i].length = lengthInSectors * tracks[i].sectorSize;
                tracks[i].offsetInFile = runningOffset;
                runningOffset += tracks[i].length;
            }

            tracks[tracks.Count - 1].offsetInFile = runningOffset;
            tracks[tracks.Count - 1].length = -1;

        }

        private void ExtractSegment(Track track)
        {
            if (!track.isAudio)
                return;

            var stream = File.OpenRead(track.file);
            if (track.length < 0)
                track.length = (int)(stream.Length - track.offsetInFile);

            var buffer = new byte[track.length];
            stream.Seek(track.offsetInFile, SeekOrigin.Begin);
            stream.Read(buffer, 0, track.length);

            var outdir = Path.GetDirectoryName(track.file) + "\\" + Path.GetFileNameWithoutExtension(track.file);
            Directory.CreateDirectory(outdir);

            var outPath = outdir + "\\track" + track.tracknumber.ToString() + ".raw";
            var outOgg = outdir + "\\track" + track.tracknumber.ToString() + ".ogg";
            if (File.Exists(outPath))
                File.Delete(outPath);
            if (File.Exists(outOgg))
                File.Delete(outOgg);

            File.WriteAllBytes(outPath, buffer);

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(
                Path.GetDirectoryName(Application.ExecutablePath) + "\\bin\\ffmpeg.exe",
                "-f s16le -ar 44100 -ac 2 -i \"" + outPath + "\" -aq 8 \"" + outOgg + "\""
                );

            app.Log("Encoding file " + outOgg + "\n");
            p.StartSilent();

            File.Delete(outPath);

        }

        private void ProcessSegments()
        {
            for (var i = 0; i < tracks.Count; i++)
            {
                ExtractSegment(tracks[i]);
            }
        }

        private Int32 parseDecInt(string v)
        {
            v = new Regex("^0*").Replace(v.Trim(), "");
            if (v == "")
                return 0;
            else
                return int.Parse(v);
        }

        private Int32 timeToOffset(string time)
        {
            //sectorsize for audio is 2352 bytes , meaning that 75 sectors are equivalent to one second
            var fields = time.Split(':');
            var mins = parseDecInt(fields[0]);
            var secs = parseDecInt(fields[1]);
            var frames = parseDecInt(fields[2]);

            var totalFrames = frames + secs * 75 + mins * 60 * 75;

            return totalFrames;
        }




        private void addSegment(string file, string trackNum, int trackOffset, int padding, int sectorSize, bool isAudio)
        {
            if (file != "" && trackNum != "")
            {
                var t = new Track();
                t.offsetInSectors = trackOffset;
                t.tracknumber = trackNum;
                t.file = file;
                t.sectorSize = sectorSize;
                t.isAudio = isAudio;
                t.preGap = padding;
                tracks.Add(t);
            }
        }

        private void SplitCue(string path)
        {
            var cue = File.ReadAllLines(path);
            var currentBin = "";
            var currentTrack = "";
            var currentOffset = 0;
            var currentGap = 0;
            var currentSectorSize = 2352;
            var currentTrackIsAudio = false;
            var foundAudioSegemnt = new List<UInt32>();
            for (var i = 0; i < cue.Length; i++)
            {
                var line = cue[i].ToUpper().Trim().Split(' ');

                if (line == null || line.Length == 0)
                    continue;

                switch (line[0])
                {
                    case "FILE":
                        if (line[line.Length - 1] == "BINARY")
                        {
                            currentBin = Path.GetDirectoryName(path) + "\\" + cue[i].TrimTo('"');

                        }
                        else
                        {
                            app.FatalExit("Non-binary CD image encountered. Not supported: " + line[line.Length - 1] + "\n");

                        }



                        break;
                    case "TRACK":
                        addSegment(currentBin, currentTrack, currentOffset, currentGap, currentSectorSize, currentTrackIsAudio);

                        currentTrack = (line[1]);


                        currentTrackIsAudio = false;
                        currentGap = 0;

                        if (line[line.Length - 1] == "AUDIO")
                        {
                            currentTrackIsAudio = true;
                            currentSectorSize = 2352;
                        }
                        else if (line[line.Length - 1] == "CDG") currentSectorSize = 2448;
                        else if (line[line.Length - 1] == "MODE1/2352") currentSectorSize = 2352;
                        else if (line[line.Length - 1] == "MODE1/2048") currentSectorSize = 2048;
                        else if (line[line.Length - 1] == "MODE2/2336") currentSectorSize = 2336;
                        else if (line[line.Length - 1] == "MODE2/2352") currentSectorSize = 2352;
                        else if (line[line.Length - 1] == "CDI/2336") currentSectorSize = 2336;
                        else if (line[line.Length - 1] == "CDI/2352") currentSectorSize = 2352;

                        break;
                    case "PREGAP":
                        currentGap = timeToOffset(line[1]);
                        break;
                    case "INDEX":
                        if (parseDecInt(line[1]) == 1)
                        {
                            currentOffset = timeToOffset(line[2]);
                        }
                        break;

                }
            }
            addSegment(currentBin, currentTrack, currentOffset, currentGap, currentSectorSize, currentTrackIsAudio);
        }



    }


}
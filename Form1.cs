using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;

namespace CueToOgg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

            button1_Click(null, null);
        }

        private void Form1_Enter(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {

            if(!File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\bin\\ffmpeg.exe"))
            {
                textBox1.AppendText("FFMPEG not found. Cannot continue.\n");
                return;
            }

            string[] files;
            files =
                Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath), "*.cue").Concat(
                Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath), "*.inst")).ToArray()
                ;
            
            if (files == null || files.Length == 0)
            {

                textBox1.AppendText("No cue sheet found in current directory. Please select the directory containing them.\n");
                Form1.ActiveForm.Invalidate();

                var pathFinder = new FolderBrowserDialog();
                var result = pathFinder.ShowDialog();
                if (result != DialogResult.OK || pathFinder.SelectedPath == null || pathFinder.SelectedPath == "" || !Directory.Exists(pathFinder.SelectedPath))
                {
                    textBox1.AppendText("No path selected. Cannot continue.\n");
                    return;
                }

                files =
                    Directory.GetFiles(pathFinder.SelectedPath, "*.cue").Concat(
                    Directory.GetFiles(pathFinder.SelectedPath, "*.inst")).ToArray()
                    ;
                
                if (files.Length == 0)
                {
                    textBox1.AppendText("No cue sheet found in path. Cannot continue.\n");
                    return;
                }
            }

            textBox1.AppendText(files.Length.ToString()+" cue sheets found in path.\n");
            if (Form1.ActiveForm != null)
                Form1.ActiveForm.Invalidate();

            for (var i = 0; i < files.Length; i++)
            {
                foundAudioSegment = new List<Track>();
                SplitCue(files[i]);
                CalcLengthAndByteOffsets();
                ProcessSegments();
            }
            textBox1.AppendText("Done\n");
            if (Form1.ActiveForm != null)
                Form1.ActiveForm.Invalidate();

        }


        private void CalcLengthAndByteOffsets()
        {
            var runningOffset = 0;
            for (var i = 0; i < (foundAudioSegment.Count-1); i++)
            {
                var nextOffset = foundAudioSegment[i + 1].offsetInSectors;
                var currentOffset= foundAudioSegment[i].offsetInSectors;
                var lengthInSectors = nextOffset - currentOffset - foundAudioSegment[i].preGap;
                foundAudioSegment[i].length = lengthInSectors * foundAudioSegment[i].sectorSize;
                foundAudioSegment[i].offsetInFile = runningOffset;
                runningOffset += foundAudioSegment[i].length;
            }

            foundAudioSegment[foundAudioSegment.Count - 1].offsetInFile = runningOffset;
            foundAudioSegment[foundAudioSegment.Count - 1].length = -1;

        }

        private void ExtractSegment(Track track)
        {
            if (!track.isAudio)
                return;

            var stream = File.OpenRead(track.file);
            if (track.length < 0)
                track.length = (int) (stream.Length - track.offsetInFile);
            
            var buffer = new byte[track.length];
            stream.Seek(track.offsetInFile, SeekOrigin.Begin);
            stream.Read(buffer, 0, track.length);

            var outdir = Path.GetDirectoryName(track.file) + "\\" + Path.GetFileNameWithoutExtension(track.file);
            Directory.CreateDirectory(outdir);

            var outPath = outdir +"\\track"+track.tracknumber.ToString()+".raw";
            var outOgg = outdir + "\\track" + track.tracknumber.ToString() + ".ogg";
            if(File.Exists(outPath))
                File.Delete(outPath);
            if (File.Exists(outOgg))
                File.Delete(outOgg);

            File.WriteAllBytes(outPath, buffer);

            var p=new Process();
            p.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath)+"\\bin\\ffmpeg.exe";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = "-f s16le -ar 44100 -ac 2 -i \"" + outPath + "\" -aq 8 \"" + outOgg + "\"";
            p.EnableRaisingEvents = true;

            textBox1.AppendText("Encoding file "+ outOgg+"\n");
            if(Form1.ActiveForm!=null)
                Form1.ActiveForm.Invalidate();

            p.Start();
            p.WaitForExit();
            File.Delete(outPath);

        }

        private void ProcessSegments()
        {
            for(var i = 0; i < foundAudioSegment.Count; i++)
            {
                ExtractSegment(foundAudioSegment[i]);
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
            var mins= parseDecInt(fields[0]);
            var secs = parseDecInt(fields[1]);
            var frames = parseDecInt(fields[2]);

            var totalFrames = frames + secs * 75 + mins * 60 * 75;

            return totalFrames;
        }



        public List<Track> foundAudioSegment;
        
        private void addSegment(string file, string trackNum, int trackOffset, int padding, int sectorSize, bool isAudio)
        {
            if(file!="" && trackNum !="")
            {
                var t = new Track();
                t.offsetInSectors = trackOffset;
                t.tracknumber = trackNum;
                t.file = file;
                t.sectorSize = sectorSize;
                t.isAudio = isAudio;
                t.preGap = padding;
                foundAudioSegment.Add(t);
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
            for(var i = 0; i < cue.Length; i++)
            {
                var line = cue[i].ToUpper().Trim().Split(' ');

                if (line == null || line.Length == 0)
                    continue;

                switch (line[0])
                {
                    case "FILE":
                        if (line[line.Length - 1] == "BINARY")
                        {
                            currentBin = Path.GetDirectoryName(path)+"\\"+(new Regex("^[^\"]*\"|\"[^\"]*$")).Replace(cue[i], "");
                            
                        }
                        else
                        {
                            currentBin = "";
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
                        currentGap= timeToOffset(line[1]);
                        break;
                    case "INDEX":
                        if (parseDecInt(line[1]) == 1) {
                            currentOffset = timeToOffset(line[2]);
                        }
                        break;

                }
            }
            addSegment(currentBin, currentTrack, currentOffset, currentGap, currentSectorSize, currentTrackIsAudio);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


    }


    public class Track
    {

        public Track()
        {

        }
        public int offsetInSectors = 0;
        public string file = "";
        public string tracknumber = "";
        public bool isAudio = false;
        public int sectorSize=0;
        public int preGap = 0;

        public int offsetInFile = 0;
        public int length = 0;
    }

}

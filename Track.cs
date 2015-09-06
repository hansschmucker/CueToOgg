
namespace CueToOgg
{
    public class Track
    {

        public Track()
        {

        }
        public int offsetInSectors = 0;
        public string file = "";
        public string tracknumber = "";
        public bool isAudio = false;
        public int sectorSize = 0;
        public int preGap = 0;

        public int offsetInFile = 0;
        public int length = 0;
    }
}

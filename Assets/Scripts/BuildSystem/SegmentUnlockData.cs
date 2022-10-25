using System;

namespace BuildSystem
{
    [Serializable]
    public class SegmentUnlockData
    {
        public string segmentName;
        public bool unlocked;

        public SegmentUnlockData(string name, bool unlocked)
        {
            segmentName = name;
            this.unlocked = unlocked;
        }

        public SegmentUnlockData(string name)
        {
            segmentName = name;
        }
    }
}
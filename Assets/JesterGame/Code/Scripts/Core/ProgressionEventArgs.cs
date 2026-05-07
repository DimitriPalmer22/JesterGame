using System;

namespace JesterGame.Code.Scripts.Core
{
    [Serializable]
    public struct ProgressionEventArgs
    {
        public int previousProgress;
        public int currentProgress;
        public int currentDay;

        public ProgressionEventArgs(int previousProgress, int currentProgress, int currentDay)
        {
            this.previousProgress = previousProgress;
            this.currentProgress = currentProgress;
            this.currentDay = currentDay;
        }
    }
}
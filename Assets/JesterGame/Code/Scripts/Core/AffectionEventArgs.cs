namespace JesterGame.Code.Scripts.Core
{
    public struct AffectionEventArgs
    {
        public string characterName;
        public int affectionValue;
        public int affectionDelta;

        public AffectionEventArgs(string characterName, int affectionValue, int affectionDelta)
        {
            this.characterName = characterName;
            this.affectionValue = affectionValue;
            this.affectionDelta = affectionDelta;
        }
    }
}
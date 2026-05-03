namespace UnrealToUnity.Code.Scripts.Core.Subsystems
{
    public class GameInstanceSubsystem : UnrealSubsystem
    {
        #region Fields

        public GameMode.GameMode CurrentGameMode { get; set; }

        #endregion

        #region Unreal Subsystem Implementation

        protected override void CustomInitialize()
        {
        }

        protected override void CustomUpdate(float deltaTime)
        {
        }

        protected override void CustomCleanUp()
        {
        }

        #endregion
    }
}
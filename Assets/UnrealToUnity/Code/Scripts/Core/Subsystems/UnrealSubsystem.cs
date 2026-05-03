using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Subsystems
{
    public abstract class UnrealSubsystem
    {
        internal void Initialize()
        {
            CustomInitialize();
        }

        protected abstract void CustomInitialize();

        internal void Update(float deltaTime)
        {
            CustomUpdate(deltaTime);
        }

        protected abstract void CustomUpdate(float deltaTime);

        internal void CleanUp()
        {
            CustomCleanUp();
        }

        protected abstract void CustomCleanUp();
    }
}
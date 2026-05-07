namespace UnrealToUnity.Code.Scripts.Core.Utility.Interfaces
{
    public interface IRunnable<in TStruct> : IRunnableBase
        where TStruct : struct
    {
        public void Run(TStruct args);
    }

    public interface IRunnableBase
    {
    }
}
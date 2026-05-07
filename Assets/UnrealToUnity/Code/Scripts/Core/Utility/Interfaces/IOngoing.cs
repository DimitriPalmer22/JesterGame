using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Utility.Interfaces
{
    public interface IOngoing<in TStruct>
        where TStruct : struct
    {
        public IEnumerator OngoingCoroutine(TStruct args);
    }
}
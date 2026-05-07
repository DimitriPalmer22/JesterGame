using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Utility.Interfaces
{
    public interface IOngoing<TType>
    {
        public IEnumerator OngoingCoroutine(TType args);
    }
}
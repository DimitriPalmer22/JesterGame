using JesterGame.Code.Scripts.Util;
using NaughtyAttributes;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters
{
    public class PawnVfxHelper : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private VfxSfxHelper stepVfxSfx;

        #endregion

        public virtual void OnStep(int step)
        {
            stepVfxSfx.Play();
        }
    }
}
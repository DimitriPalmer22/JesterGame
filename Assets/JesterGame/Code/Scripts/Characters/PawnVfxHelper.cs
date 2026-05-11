using JesterGame.Code.Scripts.Util;
using NaughtyAttributes;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters
{
    public class PawnVfxHelper : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private VfxSfxHelper stepVfxSfx;

        [SerializeField] private VfxSfxHelper affectionUpVfxSfx;
        [SerializeField] private VfxSfxHelper affectionDownVfxSfx;

        #endregion

        public virtual void OnStep(int step)
        {
            stepVfxSfx?.Play();
        }

        public void OnAffectionUp()
        {
            affectionUpVfxSfx?.Play();
        }

        public void OnAffectionDown()
        {
            affectionDownVfxSfx?.Play();
        }
    }
}
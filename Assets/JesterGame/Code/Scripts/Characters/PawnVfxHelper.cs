using NaughtyAttributes;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters
{
    public class PawnVfxHelper : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField, Foldout("Step")] private AudioSource stepAudioSource;
        [SerializeField, Foldout("Step")] private ParticleSystem stepParticle;

        #endregion

        public virtual void OnStep(int step)
        {
            stepAudioSource.Play();
            stepParticle.Play();
        }
    }
}
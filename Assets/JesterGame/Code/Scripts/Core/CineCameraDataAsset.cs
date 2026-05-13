using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace JesterGame.Code.Scripts.Core
{
    [CreateAssetMenu(fileName = "CineCameraDataAsset", menuName = "JesterGame/CineCameraDataAsset")]
    public class CineCameraDataAsset : ScriptableObject
    {
        private const int MAX_PRIORITY = 9999;
        private const int MIN_PRIORITY = -9999;

        [SerializeField] private float blendTime = 1f;

        public float BlendTime => blendTime;

        public void PrioritizeCamera(CinemachineCamera cam)
        {
            if (cam == null)
                return;

            cam.Priority = MAX_PRIORITY;
            cam.enabled = true;

            cam.Prioritize();
        }

        public void DeprioritizeCamera(CinemachineCamera cam)
        {
            if (cam == null)
                return;

            cam.Priority = MIN_PRIORITY;
            cam.enabled = false;
        }

        public IEnumerator PrioritizeCameraAndWait(CinemachineCamera cam)
        {
            PrioritizeCamera(cam);
            yield return new WaitForSeconds(blendTime);
        }

        public IEnumerator DeprioritizeCameraAndWait(CinemachineCamera cam)
        {
            DeprioritizeCamera(cam);
            yield return new WaitForSeconds(blendTime);
        }
    }
}
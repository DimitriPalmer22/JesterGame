using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnrealToUnity.Code.Scripts.Core.Levels;
using UnrealToUnity.Code.Scripts.Core.Pawns;

namespace JesterGame.Code.Scripts.Progression
{
    public class GameEndSceneLevelManager : LevelManager
    {
        [SerializeField] public PlayableDirector playableDirector;
        [SerializeField] private Transform playerPositionTransform;
        [SerializeField] private Transform impostorPositionTransform;

        public void SetPlayerAndImpostorTransforms(Pawn playerPawn, Pawn impostorPawn)
        {
            if (playerPawn != null && playerPositionTransform != null)
                SetPosition(playerPawn, playerPositionTransform);

            if (impostorPawn != null && impostorPositionTransform != null)
                SetPosition(impostorPawn, impostorPositionTransform);
        }

        private void SetPosition(Pawn pawn, Transform parent)
        {
            pawn.transform.position = Vector3.zero;
            pawn.transform.rotation = Quaternion.identity;

            pawn.transform.parent = parent;
            pawn.transform.localPosition = new Vector3(0, 1, 0);
            pawn.transform.localRotation = Quaternion.identity;
        }

        public IEnumerator PrepareCutscene()
        {
            // Get all the child game objects of the position transforms and disable them
            DisableAllChildren(playerPositionTransform);
            DisableAllChildren(impostorPositionTransform);

            yield break;
        }

        private void DisableAllChildren(Transform parent)
        {
            for (var i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.SetActive(false);
        }
    }
}
using System;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Pawns;

namespace UnrealToUnity.Code.Scripts.Core.AI
{
    public class AutoPossessor : MonoBehaviour
    {
        [SerializeField] private Pawn pawnToPossess;
        [SerializeField] private Controller controllerPrefab;

        private void Awake()
        {
            if (pawnToPossess == null || controllerPrefab == null)
                return;

            // Instantiate the controller prefab. Possess the pawn with the new controller.
            var controller = Instantiate(controllerPrefab);
            controller.Possess(pawnToPossess);
        }
    }
}
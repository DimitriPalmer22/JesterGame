using System.Collections;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Assets
{
    [CreateAssetMenu(
        fileName = "CB_PoiList_",
        menuName = "JesterGame/Character Behaviors/Point Of Interest List")
    ]
    public class PointOfInterestListBehavior : CharacterBehaviorDataAsset
    {
        [SerializeField] private string behaviorName;
        [SerializeField] private DataTableRowHandle[] pointOfInterestHandles;

        public override IEnumerator OngoingCoroutine(JesterGamePawn pawn)
        {
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                yield break;

            foreach (var handle in pointOfInterestHandles)
            {
                var pointOfInterest = gameMode.GetPointOfInterestByRowHandle(handle);

                if (pointOfInterest == null)
                    continue;

                yield return pointOfInterest.OngoingCoroutine(pawn);
            }
        }

        public override string GetBehaviorName => behaviorName;
    }
}
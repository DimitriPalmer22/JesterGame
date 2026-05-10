using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Characters.Behaviors.PointsOfInterest
{
    [CreateAssetMenu(
        fileName = "New Point of Interest Table",
        menuName = "JesterGame/Points of Interest Table")
    ]
    public class PointOfInterestTable : DataTable<int>
    {
    }
}
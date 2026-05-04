using NaughtyAttributes;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.CheatMenus;

namespace UnrealToUnity.Code.Scripts.Core.Testing
{
    public class TestCheatMenu : CheatMenu
    {
        protected override string CheatMenuTypeName => $"TestCheatMenu";

        [Button("Test Button")]
        public void TestButton()
        {
            Debug.Log($"{PlayerController.ControlledPawn.gameObject.name} is the controlled pawn.");
        }
    }
}
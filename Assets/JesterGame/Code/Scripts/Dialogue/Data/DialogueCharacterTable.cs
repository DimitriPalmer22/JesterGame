using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Dialogue.Data
{
    [CreateAssetMenu(fileName = "DialogueCharacterTable", menuName = "JesterGame/Dialogue/DialogueCharacterTable")]
    public class DialogueCharacterTable : DataTable<DialogueCharacter>
    {
    }
}
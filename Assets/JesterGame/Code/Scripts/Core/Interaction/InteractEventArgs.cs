using System;
using UnityEngine;

namespace JesterGame.Code.Scripts.Core.Interaction
{
    [Serializable]
    public struct InteractEventArgs
    {
        [SerializeField] public InteractorComponent interactor;
        [SerializeField] public InteractionHelperComponent helper;

        public InteractEventArgs(InteractorComponent interactor, InteractionHelperComponent helper)
        {
            this.interactor = interactor;
            this.helper = helper;
        }
    }
}
using System;
using UnityEngine;

namespace JesterGame.Code.Scripts.Core.Interaction
{
    [Serializable]
    public struct InteractEventArgs
    {
        [SerializeField] public InteractorComponent interactor;
        [SerializeField] public InteractionHelperComponent helper;
        [SerializeField] public InteractionHelperComponent previousHelper;

        public InteractEventArgs(InteractorComponent interactor, InteractionHelperComponent helper)
        {
            this.interactor = interactor;
            this.helper = helper;
            previousHelper = null;
        }
    }
}
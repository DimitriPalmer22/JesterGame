using System.Collections;

namespace JesterGame.Code.Scripts.Characters.Behaviors.PointsOfInterest
{
    public class EmptyPointOfInterest : PointOfInterest
    {
        protected override IEnumerator DoAtPointOfInterest(JesterGamePawn args)
        {
            yield return null;
        }
    }
}
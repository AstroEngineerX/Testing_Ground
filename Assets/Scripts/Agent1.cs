using UnityEngine;

public class Agent1 : PlayableCarrier
{
    public override void UseAbility()
    {
        Health.AffectValue(-1);
    }
}

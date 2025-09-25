using UnityEngine;

public class Agent2 : PlayableCarrier
{
    public override void UseAbility()
    {
        Health.AffectValue(-1);
    }
}
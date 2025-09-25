using UnityEngine;

public class Agent1 : PlayableCarrier
{
    public override void UseAbility()
    {
        Mana.AffectValue(-1);
    }
}
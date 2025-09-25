using UnityEngine;

public abstract class PlayableCarrier : Carrier
{
    [Header("PLAYABLE")]
    [Header("Parameters")]
    [SerializeField, Min(1)] private int maxMana = 1;
    [SerializeField] private FillType manaFillType;

    private Mana _mana;

    protected Mana Mana => _mana;

    protected override void Awake()
    {
        base.Awake();
        _mana = new Mana(0, maxMana, manaFillType);
    }

    public abstract void UseAbility();
}
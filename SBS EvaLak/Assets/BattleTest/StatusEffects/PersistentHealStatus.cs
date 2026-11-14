using UnityEngine;

public class PersistentHealStatus : StatusEffect
{
    private Healthbar target;
    private int amount;

    // Called by the card that creates this status effect
    public void Init(Healthbar target, int amount, int turns)
    {
        this.target = target;
        this.amount = amount;
        turnsLeft = turns;

        Debug.Log("Persistent heal initialzed");
    }

    protected override void OnTick()
    {
        if (target == null)
        {
            turnsLeft = 0;
            return;
        }

        target.health = Mathf.Min(target.health + amount, target.maxHealth);

        Debug.Log("Persistent heal triggered, " + turnsLeft + " turns left");
    }
}
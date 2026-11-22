using UnityEngine;

public class PersistentHealStatus : StatusEffect
{
    private int amount;

    // Called by the card that creates this status effect
    public void Init(Healthbar target, int amount, int turns)
    {
        Begin(target, turns);
        this.amount = amount;

        Debug.Log("Persistent heal initialzed");
    }

    protected override void OnTick()
    {
        if (Target == null)
        {
            turnsLeft = 0;
            return;
        }

        Target.health = Mathf.Min(Target.health + amount, Target.maxHealth);

        Debug.Log("Persistent heal triggered, " + turnsLeft + " turns left");
    }
}
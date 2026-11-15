using UnityEngine;

public class DamageOverTimeStatus : StatusEffect
{
    private Healthbar target;
    private int damagePerTick;

    // Called by the card that creates this status effect
    public void Init(Healthbar target, int amount, int turns)
    {
        this.target = target;
        damagePerTick = amount;
        turnsLeft = turns; // new DoT always resets duration

        // stack existing DoT damage instance into new one if possible
        foreach (StatusEffect effect in StatusEffect.GetAll())
        {
            if (effect == this) continue;

            if (effect is DamageOverTimeStatus other && other.target == target)
            {
                damagePerTick += other.damagePerTick;
                other.turnsLeft = 0;
                Destroy(other.gameObject);
            }
        }

        Debug.Log("DoT initialized, " + damagePerTick + " damage per turn for " + turnsLeft + " turns");
    }

    protected override void OnTick()
    {
        if (target == null)
        {
            turnsLeft = 0;
            return;
        }

        target.health = Mathf.Max(target.health - damagePerTick, 0);

        Debug.Log("DoT triggered for " + damagePerTick + ", " + (turnsLeft - 1) + " turns left");
    }
}
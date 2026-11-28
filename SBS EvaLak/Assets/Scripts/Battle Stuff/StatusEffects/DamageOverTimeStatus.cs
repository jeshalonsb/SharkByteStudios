using UnityEngine;

public class DamageOverTimeStatus : StatusEffect
{
    private int damagePerTick;

    // Called by the card that creates this status effect
    public void Init(Healthbar target, int amount, int turns)
    {
        Begin(target, turns); // new DoT always resets duration
        damagePerTick = amount;

        // stack existing DoT damage instance into new one if possible
        foreach (StatusEffect effect in StatusEffect.GetAll())
        {
            if (effect == this) continue;

            if (effect is DamageOverTimeStatus other && other.Target == Target)
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
        if (Target == null)
        {
            turnsLeft = 0;
            return;
        }

        Target.health = Mathf.Max(Target.health - damagePerTick, 0);

        Debug.Log("DoT triggered for " + damagePerTick + ", " + (turnsLeft - 1) + " turns left");
    }
}
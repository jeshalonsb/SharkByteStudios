using UnityEngine;

public class DamageOverTime : CardEffect
{
    [SerializeField] public DamageOverTimeStatus statusPrefab;
    public int damagePerTurnValue = 50;
    public int turns = 3;
    public bool targetPlayer;

    public override void Activate()
    {
        PlayModifiers mod = GameObject.FindWithTag("Modifiers").GetComponent<PlayModifiers>();

        string targetTag = targetPlayer ? "Player" : "Enemy";
        Healthbar target = GameObject.FindWithTag(targetTag).GetComponent<Healthbar>();

        DamageOverTimeStatus status = Instantiate(statusPrefab);
        status.Init(target, damagePerTurnValue, turns);

        mod.combo = 0;
    }
}
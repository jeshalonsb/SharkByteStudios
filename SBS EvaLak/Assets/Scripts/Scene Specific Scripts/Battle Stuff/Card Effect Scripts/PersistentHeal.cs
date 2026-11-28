using UnityEngine;

public class PersistentHeal : CardEffect
{
    [SerializeField] private PersistentHealStatus statusPrefab;
    public int healPerTurnValue = 50;
    public int turns = 3;
    public bool targetPlayer;

    public override void Activate()
    {
        PlayModifiers mod = GameObject.FindWithTag("Modifiers").GetComponent<PlayModifiers>();

        string targetTag = targetPlayer ? "Enemy" : "Player";
        Healthbar target = GameObject.FindWithTag(targetTag).GetComponent<Healthbar>();

        PersistentHealStatus status = Instantiate(statusPrefab);
        status.Init(target, healPerTurnValue, turns);

        mod.combo = 0;
    }
}
using UnityEngine;

public class ShieldEffect : CardEffect
{
    public bool targetPlayer;

    [SerializeField, Range(0f, 1f)] public float shieldMult = 0.5f;

    private static float playerShieldMult = 0.5f;
    private static float enemyShieldMult = 0.5f;

    public override void Activate()
    {
        PlayModifiers mod = GameObject.FindWithTag("Modifiers").GetComponent<PlayModifiers>();

        string shieldName = targetPlayer ? "PlayerShield" : "EnemyShield";

        if (targetPlayer)
            playerShieldMult = shieldMult;
        else
            enemyShieldMult = shieldMult;

        foreach (Transform t in FindObjectsByType<Transform>(FindObjectsInactive.Include))
        {
            if (t.name == shieldName)
            {
                t.gameObject.SetActive(true);
                break;
            }
        }

        mod.combo = 0;
    }

    public static float ConsumeShield(bool targetPlayer)
    {
        GameObject shield = GameObject.Find(targetPlayer ? "PlayerShield" : "EnemyShield");
        if (shield == null) return 1f;

        shield.SetActive(false);
        return targetPlayer ? playerShieldMult : enemyShieldMult;
    }
}
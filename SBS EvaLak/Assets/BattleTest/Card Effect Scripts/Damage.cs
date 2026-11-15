using UnityEngine;

public class Damage : CardEffect
{
    private EnemyManager enemyManager;
    public int damage = 50;

    public bool targetPlayer;

    public bool useDamageRange = false;
    public int minDamage = 25;
    public int maxDamage = 75;

    private float temporaryHealth;

    public void Start()
    {
        enemyManager = GameObject.FindWithTag("Enemy Manager").GetComponent<EnemyManager>();
    }
    public override void Activate()
    {
        PlayModifiers mod = GameObject.FindWithTag("Modifiers").GetComponent<PlayModifiers>();

        Healthbar HP;

        if (targetPlayer)
        {
            HP = GameObject.FindWithTag("Player").GetComponent<Healthbar>();
        }
        else
        {
            HP = GameObject.FindWithTag("Enemy").GetComponent<Healthbar>();
        }

        int finalDamage = damage;

        if (useDamageRange)
        {
            finalDamage = Random.Range(minDamage, maxDamage + 1);
        }

        temporaryHealth = HP.health;

        if (!targetPlayer)
        {
            temporaryHealth -= finalDamage * mod.damageMult;
            mod.damageMult = mod.basedamageMult;
            mod.combo = 0;
            enemyManager.PlayerAttackAnim();
        }
        else
        {
            temporaryHealth -= finalDamage;
            enemyManager.EnemyAttackAnim();
        }

        HP.health = Mathf.RoundToInt(temporaryHealth);
    }
}
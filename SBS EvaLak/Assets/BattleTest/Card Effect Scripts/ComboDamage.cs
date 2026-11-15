using UnityEngine;

public class ComboDamage : CardEffect
{
    private EnemyAnimationController enemyAnimation;

    private AnimationController playerAnimation;
    public int damage = 50;

    public bool useDamageRange = false;
    public int minDamage = 20;
    public int maxDamage = 30;

    private float temporaryHealth;
    public bool targetPlayer;

    public override void Activate()
    {
        PlayModifiers mod = GameObject.FindWithTag("Modifiers").GetComponent<PlayModifiers>();
        Healthbar enemyHP = GameObject.FindWithTag("Enemy").GetComponent<Healthbar>();
        Healthbar playerHP = GameObject.Find("PlayerHealthbar").GetComponent<Healthbar>();

        int finalDamage = damage;

        if (useDamageRange)
        {
            finalDamage = Random.Range(minDamage, maxDamage + 1);
        }

        // Each combo card played before this one doubles the damage: 1x, 2x, 4x, 8x...
        int comboMult = 1 << mod.combo;

        if (!targetPlayer) //if not targeting the player (default)
        {
            temporaryHealth = enemyHP.health;

            temporaryHealth -= finalDamage * mod.damageMult * comboMult;

            enemyHP.health = Mathf.RoundToInt(temporaryHealth);

            PlayerAttackAnim();
        }
        if (targetPlayer) //if targeting the player (enemy)
        {
            temporaryHealth = playerHP.health;

            temporaryHealth -= finalDamage * mod.damageMult * comboMult;

            playerHP.health = Mathf.RoundToInt(temporaryHealth);

            EnemyAttackAnim();
        }

        mod.damageMult = mod.basedamageMult;
        mod.combo++;
    }

    private void PlayerAttackAnim()
    {
        if (playerAnimation == null)
            playerAnimation = GameObject.FindWithTag("Player Mech").GetComponent<AnimationController>();

        if (enemyAnimation == null)
            enemyAnimation = GameObject.FindWithTag("EnemyPrefab").GetComponent<EnemyAnimationController>();

        enemyAnimation.SetHurt();
        playerAnimation.SetAttack();
    }

    private void EnemyAttackAnim()
    {
        if (playerAnimation == null)
            playerAnimation = GameObject.FindWithTag("Player Mech").GetComponent<AnimationController>();

        if (enemyAnimation == null)
            enemyAnimation = GameObject.FindWithTag("EnemyPrefab").GetComponent<EnemyAnimationController>();

        enemyAnimation.SetAttack();
    }
}
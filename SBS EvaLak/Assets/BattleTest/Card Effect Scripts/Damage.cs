using UnityEngine;

public class Damage : CardEffect
{
    private EnemyAnimationController enemyAnimation;

    private AnimationController playerAnimation;
    public int damage = 50;

    public bool targetPlayer;

    public bool useDamageRange = false;
    public int minDamage = 25;
    public int maxDamage = 75;

    private float temporaryHealth;
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
            PlayerAttackAnim();
        }
        else
        {
            temporaryHealth -= finalDamage;
            EnemyAttackAnim();
        }

        HP.health = Mathf.RoundToInt(temporaryHealth);
    }

    //move these to the enemy manager, and then you can just calculate there
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
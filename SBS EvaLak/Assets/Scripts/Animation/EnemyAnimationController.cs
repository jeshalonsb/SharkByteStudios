using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetHurt()
    {
        animator.SetBool("IsHurt", true);
    }

    public void SetAttack()
    {
        animator.SetBool("IsAttack", true);
    }
}

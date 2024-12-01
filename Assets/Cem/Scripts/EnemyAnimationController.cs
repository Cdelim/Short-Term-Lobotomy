using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private int idle = Animator.StringToHash("Idle");
    private int attack = Animator.StringToHash("Attack");
    private int walk = Animator.StringToHash("Walk");
    private int die = Animator.StringToHash("Die");
    private int damaged = Animator.StringToHash("GetDamaged");


    public void PlayIdle()
    {
        animator.SetBool(walk, false);
        animator.SetTrigger(idle);
    }

    public void Attack()
    {
        animator.SetBool(walk, false);
        animator.SetTrigger(attack);

    }

    public void Move()
    {
        animator.SetBool(walk, true);
    }

    public void Die()
    {
        animator.SetTrigger(die);
    }

    public void GetAttacked()
    {
        animator.SetBool(walk, false);
        animator.SetTrigger(damaged);

    }

}



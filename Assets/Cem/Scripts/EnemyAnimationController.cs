using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private int idle = Animator.StringToHash("Idle");
    private int attack = Animator.StringToHash("Attack");
    private int walk = Animator.StringToHash("Walk");
    private int die = Animator.StringToHash("Die");


    public void PlayIdle()
    {
        animator.SetTrigger(idle);
    }

    public void Attack()
    {
        animator.SetTrigger(attack);

    }

    public void Walk()
    {
        animator.SetBool(walk, true);
    }

    public void Die()
    {
        animator.SetTrigger(die);
    }

}



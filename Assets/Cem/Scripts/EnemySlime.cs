public class EnemySlime : EnemyBase
{


    protected override void Update()
    {
        base.Update();
    }
    protected override void Attack()
    {

    }

    protected override void Die()
    {
        base.Die();
    }

    protected override void MoveCharacter()
    {
    }

    public override void GetDamage(float damage, ElementalType type)
    {
        float damagePoint = damage * ElementalCalc.ElementalWeakness(type, enemyAttributes.elementalType);
        enemyAttributes.health -= damagePoint;
        animationController.GetAttacked();
        if (isDeath)
        {
            Die();
        }
    }
}



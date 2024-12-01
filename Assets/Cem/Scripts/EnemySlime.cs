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
        base.GetDamage(damage, type);
        
    }
}



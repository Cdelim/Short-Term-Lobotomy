using UnityEngine;
public class EnemySlime : EnemyBase
{


    protected override void Update()
    {
        base.Update();
    }
    protected override void Attack()
    {
        var attackPrefab = Utility.ObjectPool.Instance.GetFromPool(projectilePrefab);
        Vector3 dir = (Utility.WaveManager.Instance.Character.transform.position - transform.position).normalized;
        attackPrefab.transform.position = transform.position + (dir *.25f);
        attackPrefab.transform.right = dir;
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



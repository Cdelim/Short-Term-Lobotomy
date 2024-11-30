using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ElementalType
{
    Water = 0,
    Fire = 1,
    Earth = 2,
    Electric = 3,
}

[System.Serializable]
public class EnemyAttributes
{
    public float currentSpeed = 0;
    public float speedMax;
    public float acceleration;
    public float range;
    public float health;
    public float attackDeltaTime;
    public ElementalType elementalType;

    
}


public enum EnemyState
{
    Idle =0,
    Moving2Char = 1,
    Attacking = 2,
    Death = 3,
}
public abstract class EnemyBase : MonoBehaviour
{

    public EnemyAttributes enemyAttributes;
    public bool isDeath { get; private set; }

    [SerializeField]protected EnemyAnimationController animationController;

    protected EnemyState enemyState = EnemyState.Idle;
    protected float timerSec;
    protected float stopDistance;

    protected virtual void Awake()
    {
        stopDistance = Random.Range(0, enemyAttributes.range);
    }


    protected virtual void Update()
    {
//        if(GameManager.Instance.GameState != GameState.Playing) //fixme
        {
           // return;
        }

       
    }



    protected void FiniteStateMachine()
    {

        if (IsDeath())
        {
            SetEnemyState(EnemyState.Death);
            isDeath = true;
        }

        switch (enemyState)
        {
            case EnemyState.Idle:
                SetEnemyState(EnemyState.Moving2Char);
                animationController.Walk();
                break;
            case EnemyState.Moving2Char:
                enemyAttributes.currentSpeed += Time.deltaTime * enemyAttributes.acceleration;
                enemyAttributes.currentSpeed = Mathf.Clamp(enemyAttributes.currentSpeed, 0, enemyAttributes.speedMax);
                transform.position = Vector3.MoveTowards(transform.position, TargetTest.Instance.transform.position, enemyAttributes.currentSpeed * Time.deltaTime);
                if (CheckCharIsInRange())
                {
                    enemyState = EnemyState.Attacking;
                    enemyAttributes.currentSpeed = 0f;
                    animationController.Attack();
                }
                break;
            case EnemyState.Attacking:
                timerSec += Time.deltaTime;
                if(timerSec >= enemyAttributes.attackDeltaTime)
                {
                    timerSec = 0f;
                    animationController.Attack();

                }
                if (!CheckCharIsInRange())
                {
                    enemyState = EnemyState.Moving2Char;
                    animationController.Walk();
                }
                break;
            case EnemyState.Death:
                animationController.Die();
                break;
        }
    }
    protected abstract void Attack();
    protected abstract void MoveCharacter();
    protected abstract void Die();
    protected abstract void GetDamage(ElementalType type);

    protected bool IsDeath()
    {
        if(enemyAttributes.health<= 0)
        {
            return true;
        }
        return false;
    }
    protected bool CheckCharIsInRange()
    {
        if(Vector2.Distance(TargetTest.Instance.transform.position, transform.position)<= stopDistance)//todo maybe you can calculate with sqrmagnitude
        {
            return true;
        }
        return false;
    }


    protected float CalculateDamagePoint()
    {
        return 0;
    }


    protected void SetEnemyState(EnemyState enemyState)
    {
        this.enemyState = enemyState;
    }

    protected EnemyState GetEnemyState()
    {
        return enemyState;
    }
}

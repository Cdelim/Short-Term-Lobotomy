using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    ElementalType type;
    public float speed = 15.0f;
    public float damage = 1.0f;
    public float lifeTime = 5.0f;
    public bool hasCooldown = false;
    public float cooldown = 0.5f;
    public bool continues = true;

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(move());
    }

    private IEnumerator move()
    {
        if (continues)
        {
            var counter = 0.0f;
            while (counter< lifeTime)
            {
                counter += Time.deltaTime;
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                yield return null;
            }
            Destroy(gameObject);
        }
        else
        {
            var counter = 0.0f;
            var countersteps = 0.25f;
            var temp = 0.0f;
            while(counter < lifeTime)
            {
                counter+= Time.deltaTime;
                if(counter - temp >= countersteps)
                {
                    temp = counter;
                    transform.Translate(Vector2.right * speed);
                }
                yield return null;
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //other.gameObject.GetComponent<EnemyBase>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator CoolDown()
    {
        Debug.Log("Start cooldown");
        hasCooldown = true;
        yield return new WaitForSeconds(cooldown);
        hasCooldown = false;
        Debug.Log("End cooldown");
    }
}

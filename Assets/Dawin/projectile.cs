using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public ElementalType type;
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
            var counter = 0.0f;
            while (counter< lifeTime)
            {
                counter += Time.deltaTime;
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                yield return null;
            }
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyBase>().GetDamage(damage, type);
            if (!continues)
                Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<CharController>().GetDamage(damage, type);
            if (!continues)
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

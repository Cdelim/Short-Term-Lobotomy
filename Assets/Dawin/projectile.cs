using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    
    public float speed = 15.0f;
    public float damage = 1.0f;
    public float lifeTime = 5.0f;
    public bool hasCooldown = false;
    public float cooldown = 0.5f;

    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }    
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
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

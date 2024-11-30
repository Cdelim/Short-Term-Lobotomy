using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CharController : MonoBehaviour
{
    public float speed = 10.0f;
    public float dodgeVelocity = 20.0f;
    public float health = 100.0f;

    public GameObject anchor;
    public Camera cam;
    public GameObject gun;

    public bool vulnerable = true;

    public float damageMultiplier = 1.0f;

    #region Input Values

    private Vector2 moveInput;
    private Vector2 dodgeInput;
    private Vector2 lookDir;
    public bool dodging = false;

    public projectile[] projectiles;
    public int projectileIndex = 0;

    #endregion


    // Update is called once per frame
    void Update()
    {
        //the code for the rotation
        var angle = CalcDir();
        anchor.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        damageMultiplier = Mathf.Clamp(3 - lookDir.magnitude, 0.7f, 1.7f);

        //the code for the movement
        if (moveInput != Vector2.zero && !dodging)
        {
            Vector3 move = new Vector3(moveInput.x, moveInput.y, 0);
            move = move.normalized * speed * Time.deltaTime;
            transform.Translate(move, Space.Self);
        }
    }

    private float CalcDir()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        lookDir = (new Vector2(mousePos.y, mousePos.x) - new Vector2(transform.position.y, transform.position.x))
            .normalized;
        return Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
            dodgeInput = moveInput;
        }

        if (context.canceled)
        {
            moveInput = Vector2.zero;
        }
    }

    public void Dodge(InputAction.CallbackContext context)
    {
        if (context.started && vulnerable)
        {
            vulnerable = false;
            dodging = true;
            StartCoroutine(DodgeCooldown());
        }
    }

    private IEnumerator DodgeCooldown()
    {
        float counter = 0.0f;
        float duration = 0.5f;
        while (duration > counter)
        {
            counter += Time.deltaTime;
            Vector3 vel = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position,
                transform.position + new Vector3(dodgeInput.x, dodgeInput.y, 0) * dodgeVelocity, ref vel, duration);
            yield return null;
        }

        vulnerable = true;
        dodging = false;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (projectiles[projectileIndex].hasCooldown)
            {
                return;
            }

            StartCoroutine(projectiles[projectileIndex].CoolDown());
            Debug.Log("Pew!");
            projectile shot = projectiles[projectileIndex];
            float angle = CalcDir();
            shot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            shot.transform.position = gun.transform.position;
            shot.damage *= damageMultiplier;
            switch (projectileIndex)
            {
                //Fire
                case 0:
                    float spacing = 0.0f;
                    for (int i = 0; i < 50; i++)
                    {
                        projectile shots = Instantiate(shot);
                        float offset = -15f + spacing;
                        shots.transform.eulerAngles = shots.transform.eulerAngles + new Vector3(0, 0, offset);
                        float speedOffset = Random.Range(-4f, 4f);
                        shots.speed = 15.0f + speedOffset;
                        spacing = 30f / 50f * i;
                        Debug.Log(shot.transform.rotation.eulerAngles.z);
                    }

                    break;
                //Lightning
                case 1:
                    Instantiate(shot);
                    break;
                //Earth
                case 2:
                    Instantiate(shot);
                    break;
                //Water
                case 3:
                    StartCoroutine(ShootWater(shot));
                    break;
            }
        }
    }

    private IEnumerator ShootWater(projectile shot)
    {
        for (int i = 0; i < 5; i++)
        {
            projectile shots = Instantiate(shot);
            float angle = CalcDir();
            shots.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            shots.transform.position = gun.transform.position;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void SwitchElement(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //Debug.Log(context.ReadValue<Single>());
            var side = context.ReadValue<Single>();
            if (side > 0)
            {
                UIHandler.instance.RotateElement(1);
                projectileIndex++;
                if (projectileIndex == 4)
                    projectileIndex = 0;
            }
            else
            {
                UIHandler.instance.RotateElement(-1);
                projectileIndex--;
                if (projectileIndex == -1)
                {
                    projectileIndex = 3;
                }
            }
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //var damage = other.gameObject.GetComponent<Enemy>().power;
            //health -= damage
            if (health <= 0)
            {
                //Death!
            }
        }
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharController : MonoBehaviour
{
    #region Character details

    public float speed = 10.0f;
    public float dodgeVelocity = 20.0f;
    public float health = 100.0f;
    public bool vulnerable = true;
    public float damageMultiplier = 1.0f;
    
    #endregion

    #region components

    public Animator anim;
    public SpriteRenderer sprite;
    public GameObject anchor;
    public Camera cam;
    public GameObject gun;
    public GameObject rememberScreen;
    public GameObject[] images;

    public RawImage[] first;
    public RawImage[] second;
    public RawImage[] third;
    public RawImage[] fourth;

    #endregion

    #region Input Values

    private Vector2 moveInput;
    private Vector2 dodgeInput;
    private Vector2 lookDir;
    public bool dodging = false;

    public projectile[] projectiles;
    public int projectileIndex = 0;

    #endregion

    #region forgetting!

    public float[] chancesOfLobotomy = new float[4];
    public bool LobotomyTime = false;
    public float increaseRate = 0.01f;
    public int lockedElement = -1;

    public float lobotomyDuration = 15f;

    #endregion


    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        StartCoroutine(LobotomyTimer());
    }

    private IEnumerator LobotomyTimer()
    {
        while (true)
        {
            while (LobotomyTime)
            {
                yield return null;
            }
            yield return new WaitForSeconds(45f);
            var counter = 0f;
            while (counter < 15f)
            {
                counter += Time.deltaTime;
            }

            for(int i = 0; i<4;i++)
            {
                var chance = Random.Range(0.0f, 1.0f);
                if (chancesOfLobotomy[i] > chance)
                {
                    LobotomyTime = true;
                    lockedElement = i;
                    UIHandler.instance.lockElement(i, lobotomyDuration);
                    StartCoroutine(KeepLobotomy(i));
                }
            }
        }
    }

    private IEnumerator KeepLobotomy(int index)
    {
        yield return new WaitForSeconds(lobotomyDuration);
        //Slow down time for the mini game
        Time.timeScale = 0.1f;
        //mini game
        StartCoroutine(Remember(index));
    }

    private IEnumerator Remember(int i)
    {
        rememberScreen.SetActive(true);
        var index = Random.Range(1, 3);
        for (int j = 0; j < 3; j++)
        {
            if (index == j)
            {
                
            }
        }
        var counter = 0f;
        while (counter < 5f)
        {
            counter += Time.deltaTime;
            yield return null;
        }
    }

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
            if (moveInput.x < 0)
            {
                sprite.flipX=true;
            }
            else if (moveInput.x > 0)
            {
                sprite.flipX = false;
            }
            anim.SetFloat("U/D", moveInput.y);
            anim.SetFloat("L/R", moveInput.x);
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
            projectile shot = projectiles[projectileIndex];
            float angle = CalcDir();
            shot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            shot.transform.position = gun.transform.position;
            shot.damage *= damageMultiplier;
            switch (projectileIndex)
            {
                //Fire
                case 0:
                    chancesOfLobotomy[0] += increaseRate;
                    float spacing = 0.0f;
                    for (int i = 0; i < 50; i++)
                    {
                        projectile shots = Instantiate(shot);
                        float offset = -15f + spacing;
                        shots.transform.eulerAngles = shots.transform.eulerAngles + new Vector3(0, 0, offset);
                        float speedOffset = Random.Range(-4f, 4f);
                        shots.speed = 15.0f + speedOffset;
                        spacing = 30f / 50f * i;
                    }

                    break;
                //Lightning
                case 1:
                    chancesOfLobotomy[1] += increaseRate;
                    Instantiate(shot);
                    break;
                //Earth
                case 2:
                    chancesOfLobotomy[2] += increaseRate;
                    Instantiate(shot);
                    break;
                //Water
                case 3:
                    chancesOfLobotomy[3] += increaseRate;
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
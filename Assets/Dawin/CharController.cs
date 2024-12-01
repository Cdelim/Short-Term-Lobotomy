using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.U2D.Animation;
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
    public bool shootAllowed = true;
    public ElementalType currentType;
    public bool possibleLobotomy = false;

    public float fireMul = 1.0f;
    public float waterMul = 1.0f;
    public float earthMul = 1.0f;
    public float lightningMul = 1.0f;
    
    #endregion

    #region components

    public Animator anim;
    public SpriteRenderer sprite;
    public GameObject anchor;
    public Camera cam;
    public GameObject gun;
    public GameObject rememberScreen;

    public ImageButton[] first;
    public ImageButton[] second;
    public ImageButton[] third;
    public ImageButton[] fourth;
    public GameObject[] symbolMember;
    public Slider slider;
    public Slider timerSlider;
    public TextMeshProUGUI text;

    public SpriteLibrary lib;
    
    [Serializable]
    public struct ImageButton
    {
        public Image image;
        public bool isCorrect;
    }

    #endregion

    #region Input Values

    private Vector2 _moveInput;
    private Vector2 _dodgeInput;
    private Vector2 _lookDir;
    public bool dodging;

    public projectile[] projectiles;
    public int projectileIndex;

    #endregion

    #region forgetting!

    public float[] chancesOfLobotomy = new float[4];
    public bool lobotomyTime;
    public float increaseRate = 0.01f;
    public int lockedElement = -1;

    public float lobotomyDuration = 15f;

    #endregion

    private bool _escaped;

    private void Awake()
    {
        foreach (var shot in projectiles)
        {
            shot.hasCooldown = false;
        }
    }

    void Start()
    {
        StartCoroutine(LobotomyTimer());
    }

    private IEnumerator LobotomyTimer()
    {
        while (!_escaped)
        {
            while (lobotomyTime)
            {
                yield return null;
            }
            yield return new WaitForSeconds(45f);
            var counter = 0f;
            possibleLobotomy = true;
            while (counter < 15f)
            {
                counter += Time.deltaTime;
            }

            if (lobotomyTime)
            {
                yield return null;
            }
            for(int i = 0; i<4;i++)
            {
                var chance = Random.Range(0.0f, 1.0f);
                Debug.Log(chance);
                if (chancesOfLobotomy[i] > chance)
                {
                    StartCoroutine(TextDisplay(i));
                    lobotomyTime = true;
                    lockedElement = i;
                    UIHandler.instance.lockElement(i);
                    StartCoroutine(KeepLobotomy(i));
                    break;
                }
            }
        }
    }

    private IEnumerator TextDisplay(int i)
    {
        text.color = new Color(1, 1, 1, 0);
        switch (i)
        {   
            case 0:
                text.text = "LOBOTOMY TIME!\nFire!";
                break;
            case 1:
                text.text = "LOBOTOMY TIME!\nLightning!";
                break;
            case 2:
                text.text = "LOBOTOMY TIME!\nEarth!";
                break;
            case 3:
                text.text = "LOBOTOMY TIME!\nWater!";
                break;
        }

        var counter = 0f;
        while (counter < 2f)
        {
            counter+= Time.deltaTime;
            text.color = new Color(1, 1, 1, counter / 2f);
            yield return null;
        }

        while (counter > 0f)
        {
            counter-= Time.deltaTime;
            text.color = new Color(1, 1, 1, counter / 2f);
            yield return null;
        }
        text.text = "";
    }

    private IEnumerator LobotomyAnim()
    {   shootAllowed = false;
        anim.SetTrigger("Lobotomy");
        Time.timeScale = 0.1f;
        var counter = 0.0f;
        var duration = 0.5f;
        while (counter< duration)
        {
            counter += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 3.5f, counter / duration);
            yield return null;
        }
        
        cam.orthographicSize = 3.5f;
        counter = 0.0f;
        duration = 0.5f;
        while (counter< duration)
        {
            counter += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 7f, counter / duration);
            yield return null;
        }
        Time.timeScale = 1f;
        shootAllowed = true;
    }

    private IEnumerator KeepLobotomy(int index)
    {
        yield return LobotomyAnim();
        var counter = 0.0f;
        var duration = 15f;
        timerSlider.gameObject.SetActive(true);
        timerSlider.value = 0.0f;
        
        while (counter<duration)
        {
            counter += Time.deltaTime;
            timerSlider.value = counter / duration;
            yield return null;
        }
        timerSlider.gameObject.SetActive(false);
        //Slow down time for the mini game
        Time.timeScale = 0.1f;
        //mini game
        ImageButton[] images = new ImageButton[3];
        switch(index)
        {
            case 0:
                images = first;
                break;
            case 1:
                images = second;
                break;
            case 2:
                images = third;
                break;
            case 3:
                images = fourth;
                break;
        }
        StartCoroutine(Remember(index, images ));
    }

    private IEnumerator Remember(int i, ImageButton[] images)
    {
        var index = Random.Range(1, 3);
        for (int j = 0; j < 3; j++)
        {
            var temp = images[j];
            int r = Random.Range(j, 3);
            images[j] = images[r];
            images[r] = temp;
        }

        bool clicked = false;
        bool rightAnwser = false;
        for(int j = 0; j<3;j++)
        {
            symbolMember[j].GetComponent<Image>().sprite = images[j].image.sprite;
            if (images[j].isCorrect)
            {
                symbolMember[j].GetComponent<Button>().onClick.AddListener(() => rightAnwser = true);
            }
            else
            {
                symbolMember[j].GetComponent<Button>().onClick.AddListener(() => rightAnwser = false);
            }
            symbolMember[j].GetComponent<Button>().onClick.AddListener(() => clicked = true );
        }
        shootAllowed = false;
        rememberScreen.SetActive(true);
        var counter = 5f;
        while (counter > 0f&& !clicked)
        {
            counter -= Time.deltaTime;
            slider.value = counter/5f;
            yield return null;
        }
        rememberScreen.SetActive(false);
        Time.timeScale = 1f;
        if (!rightAnwser)
        {
            yield return new WaitForSeconds(2f);
            
        }
        else
        {
            StartCoroutine(BonusDamage(index));
        }
        lobotomyTime = false;
        lockedElement = -1;
        shootAllowed = true;
        chancesOfLobotomy[i] = 0.0f;
        UIHandler.instance.UnlockElement(index);
        StartCoroutine(LobotomyTimer());
    }

    private IEnumerator BonusDamage(int index)
    {
        switch (index)
        {
            case 0:
                fireMul = 1.5f;
                yield return new WaitForSeconds(10f);
                fireMul = 1.0f;
                break;
            case 1:
                lightningMul = 1.5f;
                yield return new WaitForSeconds(10f);
                lightningMul = 1.0f;
                break;
            case 2:
                earthMul = 1.5f;
                yield return new WaitForSeconds(10f);
                earthMul = 1.0f;
                break;
            case 3: 
                waterMul = 1.5f;
                yield return new WaitForSeconds(10f);
                waterMul = 1.0f;
                break;
            default:
                yield return null;
                break;
        }
        
    }
    
    // Update is called once per frame
    void Update()
    {
        //the code for the rotation
        var angle = CalcDir();
        anchor.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        damageMultiplier = Mathf.Clamp(3 - _lookDir.magnitude, 0.7f, 1.7f);

        //the code for the movement
        if (_moveInput != Vector2.zero && !dodging)
        {
            Vector3 move = new Vector3(_moveInput.x, _moveInput.y, 0);
            move = move.normalized * (speed * Time.deltaTime);
            transform.Translate(move, Space.Self);
        }
    }

    private float CalcDir()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        _lookDir = (new Vector2(mousePos.y, mousePos.x) - new Vector2(transform.position.y, transform.position.x))
            .normalized;
        return Mathf.Atan2(_lookDir.x, _lookDir.y) * Mathf.Rad2Deg;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _moveInput = context.ReadValue<Vector2>();
            if (_moveInput.x < 0)
            {
                sprite.flipX=true;
            }
            else if (_moveInput.x > 0)
            {
                sprite.flipX = false;
            }
            
            _dodgeInput = _moveInput;
        }
        if (context.canceled)
        {
            _moveInput = Vector2.zero;
            
        }
        anim.SetFloat("U/D", _moveInput.y);
        anim.SetFloat("L/R", _moveInput.x);
    }

    public void Dodge(InputAction.CallbackContext context)
    {
        if (context.started && vulnerable)
        {
            anim.SetTrigger("Dodge");
            vulnerable = false;
            dodging = true;
            shootAllowed = false;
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
                transform.position + new Vector3(_dodgeInput.x, _dodgeInput.y, 0) * dodgeVelocity, ref vel, duration);
            yield return null;
        }

        vulnerable = true;
        dodging = false;
        shootAllowed = true;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (projectiles[projectileIndex].hasCooldown || !shootAllowed)
            {
                return;
            }
            StartCoroutine(projectiles[projectileIndex].CoolDown());
            projectile shot = projectiles[projectileIndex];
            float angle = CalcDir();
            shot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            shot.transform.position = gun.transform.position;
            switch (projectileIndex)
            {
                //Fire
                case 0:
                    if (lockedElement == 0)
                    {
                        return;
                    }
                    chancesOfLobotomy[0] += increaseRate;
                    float spacing = 0.0f;
                    for (int i = 0; i < 50; i++)
                    {
                        projectile fireShot = Instantiate(shot);
                        fireShot.damage *= fireMul * damageMultiplier;
                        float offset = -30f + spacing;
                        fireShot.transform.eulerAngles = fireShot.transform.eulerAngles + new Vector3(0, 0, offset);
                        float speedOffset = Random.Range(-4f, 4f);
                        fireShot.speed = 15.0f + speedOffset;
                        spacing = 60f / 50f * i;
                    }

                    break;
                //Lightning
                case 1:
                    if (lockedElement == 1)
                    {
                        return;
                    }
                    chancesOfLobotomy[1] += increaseRate;
                    var lightningShot = Instantiate(shot);
                    lightningShot.damage *= damageMultiplier*lightningMul;
                    break;
                //Earth
                case 2:
                    if (lockedElement == 2)
                    {
                        return;
                    }
                    chancesOfLobotomy[2] += increaseRate;
                    var earthShots = Instantiate(shot);
                    earthShots.damage *= damageMultiplier*earthMul;
                    break;
                //Water
                case 3:
                    if (lockedElement == 3)
                    {
                        return;
                    }
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

    private Sprite GetSprite(int i)
    {
        switch (i)
        {
            case 0:
                return lib.GetSprite("orbs", "Fire");
            case 1:
                return lib.GetSprite("orbs", "Lightning");
            case 2:
                return lib.GetSprite("orbs", "Earth");
            case 3:
                return lib.GetSprite("orbs", "Water");
            
        }

        return null;
    }

    public void SwitchElement(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var side = context.ReadValue<Single>();
            if (side > 0)
            {
                UIHandler.instance.RotateElement(-1);
                projectileIndex++;
                if (projectileIndex == 4)
                    projectileIndex = 0;
            }
            else
            {
                UIHandler.instance.RotateElement(1);
                projectileIndex--;
                if (projectileIndex == -1)
                {
                    projectileIndex = 3;
                }
            }
            gun.GetComponent<SpriteRenderer>().sprite = GetSprite(projectileIndex);
            switch (projectileIndex)
            {
                case 0:
                    currentType = ElementalType.Fire;
                    break;
                case 1:
                    currentType = ElementalType.Electric;
                    break;
                case 2:
                    currentType = ElementalType.Earth;
                    break;
                case 3:
                    currentType = ElementalType.Water;
                    break;
            }
        }
    }
    
    public void GetDamage(float damage, ElementalType type)
    {
        if (!vulnerable)
        {
            return;
        }
        float damagePoint = damage * ElementalCalc.ElementalWeakness(type, currentType);
        health -= damagePoint;
        anim.SetTrigger("Damage");

        if (possibleLobotomy)
        {
            for(int i = 0; i<4;i++)
            {
                var chance = Random.Range(0.0f, 1.0f);
                Debug.Log(chance);
                if (chancesOfLobotomy[i] > chance)
                {
                    StartCoroutine(TextDisplay(i));
                    lobotomyTime = true;
                    lockedElement = i;
                    UIHandler.instance.lockElement(i);
                    StartCoroutine(KeepLobotomy(i));
                    break;
                }
            }
            possibleLobotomy = false;
        }
        if (health <= 0)
        {
            _escaped = true;
            //UIHandler.instance.GameOver();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator playerAnimator;
    private SpriteRenderer spriteRenderer;
    private int currentAttack = 0;
    private float timeSinceAttack = 0.0f;
    private bool isAttacking = false;
    
    [Header("Arrow Settings")]
    [SerializeField] private GameObject arrow;
    private Vector2 arrowOffset = new Vector2(0.5f, 0.005f);

    [Header("Melee Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;
    [Header("Stat Settings")]
    public float speed = 4.0f;
    public float baseATK = 714f;
    private float currentMP = 100f;
    public float maxHP = 1000f;
    public float maxMP = 40000;
    public float def = 1000f;
    private float currentHP = 1000f;
    private float mpCost = 5f;
    private float attackDamage = 25f;
    [SerializeField] private UIStats uiStats;
    [SerializeField] private float mpRegenRate = 0.4f;
    [SerializeField] private float mpRegenCooldown = 0.1f;
    private float mpRegenTimer = 0f;
    [SerializeField] private float hpRegenRate = 2f;
    [SerializeField] private float hpRegenCooldown = 0.1f;
    private float hpRegenTimer = 0f;
    [SerializeField] private float skillMPCost = 50f;
    private float critRate = 0f;
    private float critDamage = 0f;
    private bool isHurt = false;
    private bool death = false;
    private float respawnTimer = 0f;
    private float boostTimer = 0f;
    private float boostTime = 15f;
    public float skillCooldownTimer = 20f;
    private float skillCooldownTime = 20f;
    public bool isBoost = false;
    private bool isCooldown = false;
    public float hpPercentageBoost = 10f;
    private AudioSource audioSource;
    [SerializeField] private GameObject boostEffect;
    private float baseHPRegenRate = 0f;
    private float baseSpeed = 0f;
    [SerializeField] private float hpMpPercentRegenPerKill = 0.1f;
    private int enemyKillCount = 0;
    private float[] buffs = 
    {
        1f, 50f, 10f, 0.1f, 0.1f, 0.05f, 2, 4, 0.01f, 0.01f
    };

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetCritValue();
        currentHP = maxHP;
        currentMP = maxMP;
        baseHPRegenRate = hpRegenRate;
        baseSpeed = speed;
        uiStats.UpdateHP(currentHP, maxHP);
        uiStats.UpdateMP(currentMP, maxMP);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        Enemy.OnEnemyDeath += HandleEnemyDeath;
    }
    void OnDestroy()
    {
        Enemy.OnEnemyDeath -= HandleEnemyDeath;
    }
    private void SetCritValue()
    {
        critRate = CritValue.Instance.critRate;
        critDamage = CritValue.Instance.critDamage;
    }
    void Update()
    {
        if (death)
        {
            Death();
            return;
        }

        timeSinceAttack += Time.deltaTime;
        RegenerateMP();
        if (currentHP > 0) RegenerateHP();
        HandleMovementAndInput();
        Skill();
        if (Input.GetKeyDown("q"))
        {
            DamageAllEnemies();
        }
    }

    private void HandleMovementAndInput()
    {
        if (isAttacking || isHurt) return;

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        playerAnimator.SetBool("isRunning", inputX != 0 || inputY != 0);

        transform.position += new Vector3(inputX, inputY, 0) * speed * Time.deltaTime;
        spriteRenderer.flipX = inputX != 0 ? inputX < 0 : spriteRenderer.flipX;

        if (attackPoint != null)
        {
            Vector3 localPos = attackPoint.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * (spriteRenderer.flipX ? -1 : 1);
            attackPoint.localPosition = localPos;
        }

        HandleAttackInput();
    }

    void HandleAttackInput()
    {
        if (timeSinceAttack <= 0.25f) return;

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) && !isAttacking)
        {
            currentAttack = timeSinceAttack > 1f ? 1 : currentAttack % 2 + 1;
            isAttacking = true;
            playerAnimator.SetTrigger("attack" + currentAttack);
            timeSinceAttack = 0f;
        }
        else if (Input.GetMouseButtonDown(1) && !isAttacking && currentMP >= mpCost)
        {
            isAttacking = true;
            playerAnimator.SetTrigger("attack3");
            timeSinceAttack = 0f;
        }
    }

    public void Hit()
    {
        PlaySound("sword_swoosh");
        float dmgBonus = isBoost ? maxHP * hpPercentageBoost / 100 : 0;
        if (dmgBonus > baseATK * 10) dmgBonus = baseATK * 10;
        attackDamage = baseATK + dmgBonus;
        attackDamage = Mathf.Round(Random.Range(attackDamage * 0.9f, attackDamage * 1.1f) * 100f) / 100f;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>()?.OnHitByPlayer(attackDamage, critRate, critDamage);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void EndHurt()
    {
        isHurt = false;
    }

    public void shootArrow()
    {
        if (currentMP < mpCost) return;
        PlaySound("shoot_arrow");
        currentMP -= mpCost;
        uiStats.UpdateMP(currentMP, maxMP);

        float direction = spriteRenderer.flipX ? -1f : 1f;
        Vector3 spawnPosition = transform.position + new Vector3(arrowOffset.x * direction, arrowOffset.y, 0);
        GameObject arrowClone = Instantiate(arrow, spawnPosition, Quaternion.identity);

        SpriteRenderer arrowRenderer = arrowClone.GetComponent<SpriteRenderer>();
        if (arrowRenderer && direction < 0) arrowRenderer.flipX = true;

        Arrow arrowScript = arrowClone.GetComponent<Arrow>();
        arrowScript?.SetDirection(direction);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void RegenerateMP()
    {
        if (currentMP >= maxMP) return;

        mpRegenTimer += Time.deltaTime;
        if (mpRegenTimer >= mpRegenCooldown)
        {
            currentMP = Mathf.Min(currentMP + mpRegenRate, maxMP);
            mpRegenTimer = 0f;
            uiStats.UpdateMP(currentMP, maxMP);
        }
    }

    private void RegenerateHP()
    {
        if (currentHP >= maxHP) return;

        hpRegenTimer += Time.deltaTime;
        if (hpRegenTimer >= hpRegenCooldown)
        {
            currentHP = Mathf.Min(currentHP + hpRegenRate, maxHP);
            hpRegenTimer = 0f;
            uiStats.UpdateHP(currentHP, maxHP);
        }
    }

    void Death()
    {
        if (!death)
        {
            if (currentHP <= 0)
            {
                death = true;
                currentHP = currentMP = 0;
                uiStats.UpdateHP(currentHP, maxHP);
                uiStats.UpdateMP(currentMP, maxMP);
                playerAnimator.SetTrigger("death");
                respawnTimer = 0f;
                PlaySound("ouch");
            }
            return;
        }

        respawnTimer += Time.deltaTime;
        if (respawnTimer >= 3f)
        {
            death = isAttacking = isHurt = isBoost = isCooldown = false;
            boostTimer = 0f;
            skillCooldownTimer = 20f;
            currentHP = maxHP;
            currentMP = maxMP;
            hpRegenRate = baseHPRegenRate;
            speed = baseSpeed;
            uiStats.UpdateHP(currentHP, maxHP);
            uiStats.UpdateMP(currentMP, maxMP);
            transform.position = Vector3.zero;
            respawnTimer = 0f; // Reset timer sau khi tái sinh
        }
    }

    void Skill()
    {
        if (Input.GetKeyDown("e"))
        {
            if (skillCooldownTimer < skillCooldownTime)
            {
                //
            }
            else if (currentMP < skillMPCost)
            {
                //
            }
            else
            {
                boostEffect.gameObject.SetActive(true);
                currentMP -= skillMPCost;
                currentHP -= currentHP * 0.3f;
                uiStats.UpdateHP(currentHP, maxHP);
                uiStats.UpdateMP(currentMP, maxMP);
                isBoost = true;
                isCooldown = true;
                skillCooldownTimer = 0f;
                hpRegenRate *= 5;
                speed *= 1.5f;
            }
        }

        if (isBoost)
        {
            boostTimer += Time.deltaTime;
            if (boostTimer >= boostTime)
            {
                isBoost = false;
                boostTimer = 0f;
                hpRegenRate /= 5;
                speed /= 1.5f;
            }
        }
        else
        {
            boostEffect.gameObject.SetActive(false);
        }

        if (isCooldown)
        {
            skillCooldownTimer += Time.deltaTime;
            isCooldown = skillCooldownTimer < skillCooldownTime;
        }
    }

    public void TakeDamage(float amount)
    {
        if (death) return;
        amount -= def;
        if (amount <= 0)
            amount = 1;
        currentHP -= amount;
        uiStats.UpdateHP(currentHP, maxHP);
        PlaySound("takingDamage");
        if (!isBoost)
        {
            playerAnimator.SetTrigger("hurt");
            isHurt = true;
        }

        if (currentHP <= 0)
        {
            death = true;
            currentMP = 0;
            uiStats.UpdateMP(currentMP, maxMP);
            PlaySound("ouch");
            playerAnimator.SetTrigger("death");
        }
    }

    public bool ReturnDeath()
    {
        return death;
    }
    private void PlaySound(string soundName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + soundName);
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in Resources/Sounds or AudioSource is missing!");
        }
    }
    public void BowDraw()
    {
        PlaySound("bow_draw");
    }
    private void DamageAllEnemies()
    {
        if (currentMP >= 100)
        {
            currentMP -= 100;
            float aoeDamage = Mathf.Round(currentHP * 0.9f);
            currentHP -= aoeDamage;
            uiStats.UpdateHP(currentHP, maxHP);
            uiStats.UpdateMP(currentMP, maxMP);

            Enemy[] enemies = FindObjectsOfType<Enemy>();
            if (enemies.Length == 0)
            {
                return;
            }

            foreach (Enemy enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.OnHitByPlayer(aoeDamage, 0, 0); // can't crit
                }
            }
        }
    }
    public float getHPRegenRate()
    {
        return hpRegenRate;
    }
    public float getMPRegenRate()
    {
        return mpRegenRate;
    }
    private void HandleEnemyDeath(Enemy enemy)
    {
        enemyKillCount++;
        if (death) return;
        currentHP += Mathf.Round(maxHP * hpMpPercentRegenPerKill);
        if (currentHP >= maxHP) currentHP = maxHP;
        uiStats.UpdateHP(currentHP, maxHP);
        currentMP += Mathf.Round(maxMP * hpMpPercentRegenPerKill);
        if (currentMP >= maxMP) currentMP = maxMP;
        uiStats.UpdateMP(currentMP, maxMP);
    }
    public int getEnemyKillCount()
    {
        return enemyKillCount;
    }
    public void getBuff(int buffPos)
    {
        switch (buffPos)
        {
            case 0:
                baseATK += buffs[0];
                break;
            case 1:
                maxHP += buffs[1];
                currentHP += buffs[1];
                uiStats.UpdateHP(currentHP, maxHP);
                break;
            case 2:
                maxMP += buffs[2];
                currentMP += buffs[2];
                uiStats.UpdateMP(currentMP, maxMP);
                break;
            case 3:
                baseATK += baseATK * buffs[3];
                break;
            case 4:
                float tempHP = maxHP * buffs[4];
                maxHP += tempHP;
                currentHP += tempHP;
                uiStats.UpdateHP(currentHP, maxHP);
                break;
            case 5:
                float tempMP = maxMP * buffs[5];
                maxMP += tempMP;
                currentMP += tempMP;
                uiStats.UpdateMP(currentMP, maxMP);
                break;
            case 6:
                CritValue.Instance.critRate += buffs[6];
                break;
            case 7:
                CritValue.Instance.critDamage += buffs[7];
                break;
            case 8:
                baseHPRegenRate += buffs[8];
                if (isBoost) hpRegenRate += buffs[8] * 5;
                else hpRegenRate = baseHPRegenRate;
                break;
            case 9:
                mpRegenRate += buffs[9];
                break;
        }
        SetCritValue();
    }
}
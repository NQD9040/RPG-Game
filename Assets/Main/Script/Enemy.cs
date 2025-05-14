using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Enemy : MonoBehaviour
{
    [Header("HP Settings")]
    [SerializeField] private float baseHP = 100f;
    [SerializeField] private float baseDef = 10f;
    [SerializeField] private float baseATK = 1000f;

    [SerializeField] private float[] hpDefMultipliers = { 1f, 2f, 3f, 4f, 5f };
    [SerializeField] private float[] scaleMultipliers = { 1f, 1.2f, 1.4f, 1.6f, 1.8f };

    private float currentHP;
    private float def;
    private float atk;
    private int scale = 0;

    [Header("UI Elements")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private GameObject healthBarCanvas;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Transform floatingTextPoint;

    private Animator animator;
    private PlayerController player;
    private EnemyAttackHitbox attackHitbox; // Tham chiếu đến EnemyAttackHitbox

    private bool isCrit = false;
    private bool isDead = false;
    private bool isHurt = false;
    private bool isAttacking = false;
    private float hurtCooldown = 1.2f;
    private float hurtTimer = 0f;
    private Vector3 fixedPosition;
    private AudioSource audioSource;
    private int attackAnim = 1;
    public static event Action<Enemy> OnEnemyDeath;
    void Start()
    {
        // Randomly determine the scale level
        scale = UnityEngine.Random.Range(0, hpDefMultipliers.Length);

        // Apply scale and HP/DEF multipliers
        float scaleFactor = scaleMultipliers[scale];
        float hpFactor = hpDefMultipliers[scale];

        currentHP = baseHP * hpFactor;
        def = baseDef * hpFactor;
        atk = baseATK * scaleFactor;

        // Scale the enemy visually
        transform.localScale *= scaleFactor;

        // Initialize components
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        attackHitbox = GetComponentInChildren<EnemyAttackHitbox>();

        // Update the health bar UI
        UpdateHealthBar();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Handle hurt state
        if (isHurt && !isDead)
        {
            hurtTimer += Time.deltaTime;
            if (hurtTimer >= hurtCooldown)
            {
                isHurt = false;
            }
        }

        // Lock movement during attack or hurt state
        if ((isAttacking || isHurt) && !isDead)
        {
            transform.position = fixedPosition;
        }

        // Keep the health bar facing the camera
        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.rotation = Quaternion.identity;
        }
    }

    public void OnHitByPlayer(float damage, float critRate, float critDamage)
    {
        if (isDead) return;

        fixedPosition = transform.position;
        isHurt = true;
        hurtTimer = 0f;

        // Determine if the hit is a critical hit
        isCrit = UnityEngine.Random.Range(0, 100) <= critRate && critRate != 0;

        if (isCrit)
        {
            damage += damage * (critDamage / 100f);
        }

        damage = Mathf.Max(1f, Mathf.Round((damage - def) * 100f) / 100f);

        ShowDamageText(damage);
        ApplyDamage(damage);
    }

    private void ApplyDamage(float damage)
    {
        animator.SetTrigger("hurt");
        PlaySound("enemy_hit");
        currentHP = Mathf.Clamp(currentHP - damage, 0, currentHP);
        UpdateHealthBar();

        if (currentHP <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
    }

    private void ShowDamageText(float damage)
    {
        if (!floatingTextPrefab || !floatingTextPoint) return;

        GameObject textObj = Instantiate(floatingTextPrefab, floatingTextPoint.position, Quaternion.identity);
        FloatingText ft = textObj.GetComponent<FloatingText>();
        if (ft != null)
        {
            Color color = isCrit ? Color.blue : Color.black;
            ft.SetText("-" + damage.ToString(), color);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHP / (baseHP * hpDefMultipliers[scale]);
        }
    }

    private void Die()
    {
        animator.SetTrigger("death");
    }

    public void DestroyAfterDeath()
    {
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject);
    }

    public void Attack()
    {
        if (player != null && !isHurt && !isDead)
        {
            if (attackAnim == 1) attackAnim = 2;
            else if (attackAnim == 2) attackAnim = 1;
            fixedPosition = transform.position;
            isAttacking = true;
            isHurt = true;
            hurtTimer = 0f;
            animator.SetTrigger("attack" + attackAnim);
        }
    }

    public void attack()
    {
        PlaySound("sword_swoosh");
        if (attackHitbox != null && attackHitbox.IsPlayerInHitbox() && player != null)
        {
            player.TakeDamage(atk);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public int ReturnScale()
    {
        return scale;
    }

    public bool IsDead()
    {
        return isDead;
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
}
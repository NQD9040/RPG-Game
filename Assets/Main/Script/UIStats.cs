using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStats : MonoBehaviour
{
    [SerializeField] private Image deathImg;
    [Header("HP Settings")]
    [SerializeField] private Image hpBar;
    private float targetHpFill = 1f;

    [Header("MP Settings")]
    [SerializeField] private Image mpBar;
    private float targetMpFill = 1f;
    [Header("Text Setting")]
    [SerializeField] private TextMeshProUGUI skillCooldownText;
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI MPText;
    [SerializeField] private TextMeshProUGUI enemyKillCountText;
    [SerializeField] private float smoothSpeed = 10f;
    private PlayerController cPlayer;
    private void Start()
    {
        cPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    private void Update()
    {
        if (hpBar != null)
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, targetHpFill, Time.deltaTime * smoothSpeed);

        if (mpBar != null)
            mpBar.fillAmount = Mathf.Lerp(mpBar.fillAmount, targetMpFill, Time.deltaTime * smoothSpeed);
        
        float skillCooldown = Mathf.Round((20 - cPlayer.skillCooldownTimer) * 100f) / 100f;
        if (skillCooldown <= 0)
            skillCooldown = 0;
        skillCooldownText.text = "Skill Cooldown: " + skillCooldown;
        if (cPlayer.ReturnDeath())
            deathImg.gameObject.SetActive(true);
        if (!cPlayer.ReturnDeath())
            deathImg.gameObject.SetActive(false);
        enemyKillCountText.text = "Enemy Killed : " + cPlayer.getEnemyKillCount();
    }
    
    public void UpdateHP(float currentHP, float maxHP)
    {
        if (currentHP <= 0)
            currentHP = 0;
        HPText.text = Mathf.Round(currentHP) + "/" + Mathf.Round(maxHP);
        targetHpFill = Mathf.Clamp01(currentHP / maxHP);
    }

    public void UpdateMP(float currentMP, float maxMP)
    {
        MPText.text = Mathf.Round(currentMP) + "/" + Mathf.Round(maxMP);
        targetMpFill = Mathf.Clamp01(currentMP / maxMP);
    }
}

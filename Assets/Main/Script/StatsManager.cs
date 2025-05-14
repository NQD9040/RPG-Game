using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StatsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI maxHPText;
    [SerializeField] private TextMeshProUGUI maxMPText;
    [SerializeField] private TextMeshProUGUI baseATKText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI hpRegenRateText;
    [SerializeField] private TextMeshProUGUI mpRegenRateText;
    [SerializeField] private TextMeshProUGUI critRateText;
    [SerializeField] private TextMeshProUGUI critDMGText;
    private PlayerController playerCon;
    void Start()
    {
        playerCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        maxHPText.text = "Max HP : " + Mathf.Round(playerCon.maxHP);
        maxMPText.text = "Max MP : " + Mathf.Round(playerCon.maxMP);
        baseATKText.text = "Base ATK : " + Mathf.Round(playerCon.baseATK);
        defText.text = "Def : " + playerCon.def;
        speedText.text = "Speed : " + playerCon.speed;
        hpRegenRateText.text = "HP Regen Rate : " + Mathf.Round(playerCon.getHPRegenRate() * 10000f)/100f + "/s";
        mpRegenRateText.text = "MP Regen Rate : " + Mathf.Round(playerCon.getMPRegenRate() * 10000f)/100f + "/s";
        critRateText.text = "Crit Rate : " + CritValue.Instance.critRate + "%";
        critDMGText.text = "Crit Dmg : " + CritValue.Instance.critDamage + "%";
    }
}

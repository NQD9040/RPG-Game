using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectManager : MonoBehaviour
{
    private string[] buff = 
    {"Base ATK + 1",
     "Max HP + 50",
     "Max MP + 10",
     "Base ATK + 10%",
     "Max HP + 10%",
     "Max MP + 5%",
     "Crit Rate + 2%",
     "Crit Damage + 4%",
     "HP Regen Rate + 1/s",
     "MP Regen Rate + 1/s"};
    [SerializeField] private Button selection1;
    [SerializeField] private Button selection2;
    [SerializeField] private Button selection3;
    [SerializeField] private Canvas canvas;
    private PlayerController playerController;
    private bool isOpenSelection = false;
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (selection1 != null && selection2 != null && selection3 != null) 
        {
            selection1.onClick.AddListener(ToggleSelection);
            selection2.onClick.AddListener(ToggleSelection);
            selection3.onClick.AddListener(ToggleSelection);
        }
        SetTextSelection();
    }
    void Update()
    {
        
    }
    public void SetTextSelection()
    {
        int rand = Random.Range(0, buff.Length);
        SetButtonText(selection1, buff[rand]);
        rand = Random.Range(0, buff.Length);
        SetButtonText(selection2, buff[rand]);
        rand = Random.Range(0, buff.Length);
        SetButtonText(selection3, buff[rand]);
    }
    private void SetButtonText(Button button, string text)
    {
        if (button == null)
        {
            Debug.LogWarning("Button is not assigned!");
            return;
        }
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = text;
        }
        else
        {
            Debug.LogWarning($"No TMP_Text found in children of button {button.name}!");
        }
    }
    private void ToggleSelection()
    {

        Button clickedButton = EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
        if (clickedButton == null)
        {
            Debug.LogWarning("Không thể xác định nút được nhấn!");
            return;
        }

        TMP_Text buttonText = clickedButton.GetComponentInChildren<TMP_Text>();
        if (buttonText == null)
        {
            Debug.LogWarning("Không tìm thấy TMP_Text trên nút được nhấn!");
            return;
        }

        int buffPos = -1;
        for (int i = 0; i < buff.Length; i++)
        {
            if (buttonText.text == buff[i])
            {
                buffPos = i;
                break;
            }
        }

        if (buffPos == -1)
        {
            Debug.LogWarning($"Không tìm thấy buff khớp với text: {buttonText.text}");
            return;
        }
        playerController.getBuff(buffPos);
        canvas.gameObject.SetActive(false);
        isOpenSelection = false;
        Time.timeScale = 1f;
        SetTextSelection();
    }
    public void SetOpenSelectTrue()
    {
        isOpenSelection = true;
    }
    public bool IsOpenSelectTrue()
    {
        return isOpenSelection;
    }
}

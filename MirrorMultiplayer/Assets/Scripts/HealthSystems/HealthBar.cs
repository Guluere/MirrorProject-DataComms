using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public Image healthBar;

    public HealthSystem healthSystem;

    private void Start()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        float HealthCut = healthSystem.Health / (float)healthSystem.MaxHealth.FullValue;
        Debug.Log(HealthCut);
        float HealthPercentage = HealthCut * 100; 
        Debug.Log(HealthPercentage);
        healthText.text = "Health: " + (int)HealthPercentage + "%";
        healthBar.fillAmount = HealthCut;
    }
}
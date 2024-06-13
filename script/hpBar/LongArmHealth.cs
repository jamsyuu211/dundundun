using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LongArmHealth : MonoBehaviour
{
    float maxHealth;
    float currentHealth;
    public Image healthBar; // 체력 바 이미지
    public Color fullHealthColor = Color.red; // 체력이 많을 때 색상
    public Color lowHealthColor = Color.black; // 체력이 적을 때 색상

    LongArm LAScript;

    public void publicReset()
    {
        maxHealth = LAScript.hp;
        currentHealth = maxHealth;
        healthBar.fillAmount = 1.0f;

        UpdateHealthBar();
    }
    void Start()
    {
        healthBar.fillAmount = 1.0f; // 초기화

        LAScript = GetComponent<LongArm>();
        maxHealth = LAScript.hp;
        currentHealth = maxHealth;

        UpdateHealthBar();
    }

    // 데미지를 받았을 때 호출되는 함수
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // 체력 바 업데이트 함수
    void UpdateHealthBar()
    {
        // fillAmount 속성을 사용하여 체력 바의 크기를 조정
        healthBar.fillAmount = currentHealth / maxHealth;

        // 체력에 따라 체력 바의 색상을 변경
        healthBar.color = Color.Lerp(lowHealthColor, fullHealthColor, currentHealth / maxHealth);
    }
}

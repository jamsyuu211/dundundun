using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    float maxHealth;
    float currentHealth;
    public Image healthBar; // 체력 바 이미지
    public Color fullHealthColor = Color.green; // 체력이 많을 때 색상
    public Color lowHealthColor = Color.red; // 체력이 적을 때 색상

    playerMove playerScript;

    public void publicReset()
    {
        healthBar.fillAmount = 1.0f;
        maxHealth = playerScript.maxHp;
        currentHealth = playerScript.hp;

        UpdateHealthBar();
    }
    void Start()
    {
        healthBar.fillAmount = 1.0f; // 초기화

        playerScript = GetComponent<playerMove>();
        maxHealth = playerScript.maxHp;
        currentHealth = playerScript.hp;

        UpdateHealthBar();
    }
    
    // 데미지를 받았을 때 호출되는 함수
    public void TakeDamage()
    {
        currentHealth = playerScript.hp;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // 체력 바 업데이트 함수
    public void UpdateHealthBar()
    {
        currentHealth = playerScript.hp;
        maxHealth = playerScript.maxHp;
        // fillAmount 속성을 사용하여 체력 바의 크기를 조정
        healthBar.fillAmount = currentHealth / maxHealth;

        // 체력에 따라 체력 바의 색상을 변경
        healthBar.color = Color.Lerp(lowHealthColor, fullHealthColor, currentHealth / maxHealth);

        playerScript.textHp.text = playerScript.hp.ToString();
    }
}

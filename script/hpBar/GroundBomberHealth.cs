using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundBomberHealth : MonoBehaviour
{
    float maxHealth;
    float currentHealth;
    public Image healthBar; // ü�� �� �̹���
    public Color fullHealthColor = Color.red; // ü���� ���� �� ����
    public Color lowHealthColor = Color.black; // ü���� ���� �� ����

    GroundBomber GBScript;

    public void publicReset()
    {
        maxHealth = GBScript.hp;
        currentHealth = maxHealth;
        healthBar.fillAmount = 1.0f;

        UpdateHealthBar();
    }
    void Start()
    {
        healthBar.fillAmount = 1.0f; // �ʱ�ȭ

        GBScript = GetComponent<GroundBomber>();
        maxHealth = GBScript.hp;
        currentHealth = maxHealth;

        UpdateHealthBar();
    }

    // �������� �޾��� �� ȣ��Ǵ� �Լ�
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // ü�� �� ������Ʈ �Լ�
    void UpdateHealthBar()
    {
        // fillAmount �Ӽ��� ����Ͽ� ü�� ���� ũ�⸦ ����
        healthBar.fillAmount = currentHealth / maxHealth;

        // ü�¿� ���� ü�� ���� ������ ����
        healthBar.color = Color.Lerp(lowHealthColor, fullHealthColor, currentHealth / maxHealth);
    }
}
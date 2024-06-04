using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    float maxHealth;
    float currentHealth;
    public Image healthBar; // ü�� �� �̹���
    public Color fullHealthColor = Color.green; // ü���� ���� �� ����
    public Color lowHealthColor = Color.red; // ü���� ���� �� ����

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
        healthBar.fillAmount = 1.0f; // �ʱ�ȭ

        playerScript = GetComponent<playerMove>();
        maxHealth = playerScript.maxHp;
        currentHealth = playerScript.hp;

        UpdateHealthBar();
    }
    
    // �������� �޾��� �� ȣ��Ǵ� �Լ�
    public void TakeDamage()
    {
        currentHealth = playerScript.hp;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // ü�� �� ������Ʈ �Լ�
    public void UpdateHealthBar()
    {
        currentHealth = playerScript.hp;
        maxHealth = playerScript.maxHp;
        // fillAmount �Ӽ��� ����Ͽ� ü�� ���� ũ�⸦ ����
        healthBar.fillAmount = currentHealth / maxHealth;

        // ü�¿� ���� ü�� ���� ������ ����
        healthBar.color = Color.Lerp(lowHealthColor, fullHealthColor, currentHealth / maxHealth);

        playerScript.textHp.text = playerScript.hp.ToString();
    }
}

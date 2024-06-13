using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    float maxHealth;
    float currentHealth;
    public Image healthBar; // ü�� �� �̹���
    public Light2D objLight;
    public Color lightColor = new Color(1f, 1f, 1f, 0.3f);
    public float ScaleNum = 10f;
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
        float ratio = currentHealth / maxHealth;

        //�÷��̾��� hp�� ���� light ���� ����
        objLight.color = Color.Lerp(lightColor, Color.white, ratio);
        
        // fillAmount �Ӽ��� ����Ͽ� ü�� ���� ũ�⸦ ����
        healthBar.fillAmount = ratio;

        // ü�¿� ���� ü�� ���� ������ ����
        healthBar.color = Color.Lerp(lowHealthColor, fullHealthColor, ratio);

        playerScript.textHp.text = playerScript.hp.ToString("F1");
    }
}

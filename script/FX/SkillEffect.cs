using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    [SerializeField] Camera mainCamera; // ĸó�� ī�޶�
    [SerializeField] Material screenMaterial; // ȭ�鿡 ������ ��Ƽ���� (�Ϸ��̴� ȿ���� ���� ���̴��� ����� ��Ƽ����)
    [SerializeField] float freezeDuration = 2f; // ȭ���� ���ߴ� �ð�
    [SerializeField] GameObject skillObj;
    [SerializeField] GameObject objFlow;
    [SerializeField] playerMove playerScript;
    
    AudioSource objAudio;
    private RenderTexture renderTexture;
    private Texture2D screenTexture;
    public bool isFreezing = false;

    private float disolveScaleVar = 0f;
    private float disolveScale = 0.9f;
    private bool isMin = false;
    private bool IsCoroutineRunning = false;

    //��Ÿ�� ���� ����
    public float FlowT = 0f;
    public float FlowDuration = 3f;

    public void publlicReset()
    {
        isFreezing = false;

        disolveScaleVar = 0f;
        disolveScale = 0.9f;
        isMin = false;
        IsCoroutineRunning = false;

        FlowT = 0f;
    }
    private void Start()
    {
        objAudio = GetComponent<AudioSource>();
        // �ʱ�ȭ���� RenderTexture �� Texture2D ����
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        objAudio.mute = true;
        objFlow.SetActive(false);
        screenMaterial.SetFloat("_DisolveScale", 0.9f);
    }
    private void OnDestroy()
    {
        // �޸� ������ �����ϱ� ���� ��ü �ı� ������ ����
        Destroy(renderTexture);
        Destroy(screenTexture);
    }

    private void Update()
    {
        if (playerScript.hp > 0f)
        {
            if (Input.GetKeyDown(KeyCode.R) && !isFreezing)
            {
                objAudio.mute = false;
                objFlow.SetActive(true);
                objAudio.Play();
                StartCoroutine(WaitForSeconds(0.5f));
                UseSkill();
            }
        }

        if (isFreezing)
        {
            FlowT += Time.deltaTime;
            if(FlowT >= FlowDuration)
            {
                FlowT = 0f;
                isFreezing = false;
            }
        }

        if (skillObj.activeInHierarchy)
        {
            float currentDisolveScale = screenMaterial.GetFloat("_DisolveScale");

            if (currentDisolveScale <= 0.9f)
            {
                isMin = true;
            }
            else if (currentDisolveScale >= 1f)
            {
                isMin = false;
            }

            if (isMin)
            {
                if (!IsCoroutineRunning)
                {
                    StartCoroutine(UpdateDisolveScale());
                }
            }
            else
            {
                ResetDisolveScale();
            }
        }
    }
    IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
    }
    public void UseSkill()
    {
        StartFreeze();
    }

    private void StartFreeze()
    {
        // ī�޶��� Ÿ�� �ؽ�ó�� RenderTexture�� ����
        mainCamera.targetTexture = renderTexture;

        // RenderTexture���� �ؽ�ó�� ��ȯ
        mainCamera.Render();
        RenderTexture.active = renderTexture;
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        // ī�޶��� Ÿ�� �ؽ�ó�� ������� ����
        mainCamera.targetTexture = null;
        RenderTexture.active = null;

        // ȭ�鿡 �ؽ�ó ����
        screenMaterial.SetTexture("_GrabTexture", screenTexture);  // ���⼭ _MainTex�� ���̴��� �°� ���� �ʿ�

        // ȭ���� ���ߴ� ȿ�� ����
        isFreezing = true;

        skillObj.SetActive(true);
        screenMaterial.SetFloat("_DisolveScale", 0.9f);
    }
    private IEnumerator UpdateDisolveScale()
    {
        IsCoroutineRunning = true;
        while (isMin && gameObject.activeInHierarchy)
        {
            disolveScaleVar += Time.deltaTime * 0.3f;
            disolveScale += Mathf.Lerp(disolveScaleVar, 1f, Time.deltaTime * 0.3f);
            screenMaterial.SetFloat("_DisolveScale", disolveScale);
            yield return new WaitForSeconds(0.07f);
        }
        IsCoroutineRunning = false;
    }

    private void ResetDisolveScale()
    {
        objFlow.SetActive(false);
        disolveScaleVar = 0f;
        disolveScale = 0.9f;
        screenMaterial.SetFloat("_DisolveScale", 0.9f);
        screenMaterial.SetTexture("_GrabTexture", null);
        skillObj.SetActive(false);
    }
}

using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    [SerializeField] Camera mainCamera; // 캡처할 카메라
    [SerializeField] Material screenMaterial; // 화면에 적용할 머티리얼 (일렁이는 효과를 위한 셰이더가 적용된 머티리얼)
    [SerializeField] float freezeDuration = 2f; // 화면이 멈추는 시간
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

    //쿨타임 관련 변수
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
        // 초기화에서 RenderTexture 및 Texture2D 생성
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        objAudio.mute = true;
        objFlow.SetActive(false);
        screenMaterial.SetFloat("_DisolveScale", 0.9f);
    }
    private void OnDestroy()
    {
        // 메모리 누수를 방지하기 위해 객체 파괴 시점에 정리
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
        // 카메라의 타겟 텍스처를 RenderTexture로 설정
        mainCamera.targetTexture = renderTexture;

        // RenderTexture에서 텍스처로 변환
        mainCamera.Render();
        RenderTexture.active = renderTexture;
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        // 카메라의 타겟 텍스처를 원래대로 복구
        mainCamera.targetTexture = null;
        RenderTexture.active = null;

        // 화면에 텍스처 적용
        screenMaterial.SetTexture("_GrabTexture", screenTexture);  // 여기서 _MainTex는 셰이더에 맞게 변경 필요

        // 화면이 멈추는 효과 시작
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

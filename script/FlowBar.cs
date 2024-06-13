using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.UI;

public class FlowBar : MonoBehaviour
{
    [SerializeField] SkillEffect FlowScript;
    Image skillBar;
    float BarRatio = 0f;
    public void publicReset()
    {
        skillBar.fillAmount = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        skillBar = GetComponent<Image>();
        skillBar.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(FlowScript.isFreezing)
        {
            updateSkillBar();
        }
    }

    void updateSkillBar()
    {
        BarRatio = 1f - (FlowScript.FlowT / FlowScript.FlowDuration);

        skillBar.fillAmount = BarRatio;
    }
}

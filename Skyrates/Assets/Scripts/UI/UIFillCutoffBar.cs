using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIFillCutoffBar : MonoBehaviour
{

    public Canvas Owner;
    public Image Empty;
    public Image Full;
    public Image Cutoff;

    private RectTransform _rect;

    void Awake()
    {
        this._rect = this.GetComponent<RectTransform>();
        this.Empty.enabled = false;
        this.Full.enabled = false;
        this.Cutoff.enabled = false;
        this.Full.rectTransform.sizeDelta = this._rect.sizeDelta;
    }

    public void Execute(float cutoffStart, float cutoffEnd, Func<float> getAmountComplete)
    {
        StartCoroutine(this.FillBar(cutoffStart, cutoffEnd, getAmountComplete));
    }

    private IEnumerator FillBar(float cutoffStart, float cutoffEnd, Func<float> getAmountComplete)
    {
        this.SetCutoff(cutoffStart, cutoffEnd);
        this.Full.fillAmount = 0.0f;

        this.Empty.enabled = true;
        this.Full.enabled = true;
        this.Cutoff.enabled = true;

        while (this.Full.fillAmount < 1.0f)
        {
            this.Full.fillAmount = Mathf.Min(1.0f, getAmountComplete());
            yield return null;
        }
        
        this.Empty.enabled = false;
        this.Full.enabled = false;
        this.Cutoff.enabled = false;

        this.SetCutoff(0.0f, 0.0f);
        this.Full.fillAmount = 0.0f;
    }

    // assume: percent [0, 1]
    public void SetCutoff(float percentStart, float percentEnd)
    {
        this.Cutoff.rectTransform.position = new Vector3(
            this.Empty.rectTransform.position.x + this.Owner.scaleFactor * this._rect.sizeDelta.x * 0.2f,
            this._rect.position.y,
            this._rect.position.z);
        this.Cutoff.rectTransform.sizeDelta = new Vector2(
            this._rect.sizeDelta.x * (percentEnd - percentStart),
            this._rect.sizeDelta.y
        );
    }

}

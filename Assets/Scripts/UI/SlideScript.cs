using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlideScript : MonoBehaviour
{
    public SlideType slideType;
    public float duration = 0.3f;
    public float delay = 0;
    public float forcedOffset = 0;
    RectTransform rt;
    float startPoint;
    float offset;
    // Start is called before the first frame update
    void Start()
    {
        _slideButton = transform.Find("Slide Button");
        GameManager.Instance._onVictory += HidePanel;
        GameManager.Instance._onResetElements += ActivePanel;

        rt = this.GetComponent<RectTransform>();
        int sign = 1;
        switch (slideType) {
            case SlideType.down:
                sign = -1;
                goto case SlideType.up;
            case SlideType.up:
                startPoint = rt.anchoredPosition.y;
                offset = sign * rt.rect.height;
                break;

            case SlideType.left:
                sign = -1;
                goto case SlideType.right;
            case SlideType.right:
                startPoint = rt.anchoredPosition.x;
                offset = sign * rt.rect.width;
                break;
        }
        /*if (slideType.Equals(SlideType.up) || slideType.Equals(SlideType.down)) {
            startPoint = rt.anchoredPosition.y;
            offset = (int)slideType * rt.rect.height;
        } else {
            startPoint = rt.anchoredPosition.x;
            offset = (int)slideType * rt.rect.width;
        }*/

        if (forcedOffset != 0) offset = forcedOffset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void Show(float duration = 0f, float delay = 0f)
    {
        float distance;
        if (slideType.Equals(SlideType.up) || slideType.Equals(SlideType.down)) distance = rt.rect.height;
        else distance = rt.rect.width;

        rt.DOAnchorPosY((int)slideType * distance, duration).SetDelay(delay);
    }

    public void Hide(float duration = 0f, float delay = 0f)
    {
        float distance;
        if (slideType.Equals(SlideType.up) || slideType.Equals(SlideType.down)) distance = rt.rect.height;
        else distance = rt.rect.width;

        rt.DOAnchorPosY(-1 * (int)slideType * rt.rect.height, duration).SetDelay(delay);
    }*/

    public void Slide()
    {
        float endPoint = startPoint + offset;

        if (slideType.Equals(SlideType.up) || slideType.Equals(SlideType.down)) rt.DOAnchorPosY(endPoint, duration).SetDelay(delay);
        else rt.DOAnchorPosX(endPoint, duration).SetDelay(delay);

        offset *= -1;
        _isSlided = !_isSlided;
        startPoint = endPoint;
    }

    public void HidePanel() {
        if (_isSlided) Slide();
        _slideButton.gameObject.SetActive(false);
    }

    public void ActivePanel() {
        _slideButton.gameObject.SetActive(true);
    }

    public enum SlideType : int {up,down,left,right};
    private bool _isSlided = false;
    private Transform _slideButton;
}

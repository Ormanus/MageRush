using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    public GameObject[] PageObjects;
    public Button ButtonForward;
    public Button ButtonBack;
    public Text Counter;
    public int CurrentPageIndex = 0;

    const float SwitchDuration = 0.1f;
    [System.Serializable]
    public class Page
    {
        public GameObject go;
        public Vector3 fullScale;
        public int index;
        public float rightBorder;
        public float leftBorder;
        public RectTransform rt;

        public void SetWidth(float width)
        {
            if (go == null)
                return;

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.localScale = new Vector3(width * fullScale.x, fullScale.y, fullScale.z);
        }

        public Page(GameObject gameObj, int id)
        {
            go = gameObj;
            fullScale = Vector3.one;
            index = id;
            rt = go.GetComponent<RectTransform>();
            leftBorder = -rt.rect.width / 2;
            rightBorder = rt.rect.width / 2;
        }
    }
    Page[] _pages;
    public enum PageAction
    {
        Forward,
        Back
    }
    enum AnimationType
    {
        AppearFromRight,
        AppearFromLeft,
        DisappearToRight,
        DisappearToLeft,
        Reset
    }
    [System.Serializable]
    class PageAnimation
    {
        Page page_;
        AnimationType animationType_;
        float phase_;

        public bool UpdatePage(float deltaTime)
        {
            if (page_ == null)
                return false;

            phase_ += deltaTime / SwitchDuration;
            if (phase_ < 0)
                return true;

            if (phase_ > 1)
            {
                switch (animationType_)
                {
                    case AnimationType.AppearFromLeft:
                    case AnimationType.AppearFromRight:
                        page_.rt.anchoredPosition = new Vector2(0, page_.rt.anchoredPosition.y);
                        page_.SetWidth(1);
                        break;
                    case AnimationType.DisappearToLeft:
                    case AnimationType.DisappearToRight:
                        page_.SetWidth(0);
                        break;
                }
                return false;
            }

            switch (animationType_)
            {
                case AnimationType.AppearFromRight:
                    //page_.rt.pivot = new Vector2(1 - phase_ / 2, 0.5f);
                    page_.rt.anchoredPosition = new Vector2(page_.rightBorder * (1 - phase_ * phase_), page_.rt.anchoredPosition.y);
                    break;
                case AnimationType.AppearFromLeft:
                    //page_.rt.pivot = new Vector2(phase_ / 2, 0.5f);
                    page_.rt.anchoredPosition = new Vector2(page_.leftBorder * (1 - phase_ * phase_), page_.rt.anchoredPosition.y);
                    break;
                case AnimationType.DisappearToRight:
                    //page_.rt.pivot = new Vector2(0.5f + phase_ / 2, 0.5f);
                    page_.rt.anchoredPosition = new Vector2(page_.rightBorder * phase_ * phase_, page_.rt.anchoredPosition.y);
                    break;
                case AnimationType.DisappearToLeft:
                    //page_.rt.pivot = new Vector2(0.5f - phase_/2, 0.5f);
                    page_.rt.anchoredPosition = new Vector2(page_.leftBorder * phase_ * phase_, page_.rt.anchoredPosition.y);
                    break;
            }

            switch (animationType_)
            {
                case AnimationType.AppearFromLeft:
                case AnimationType.AppearFromRight:
                    page_.SetWidth(phase_ * phase_);
                    break;
                case AnimationType.DisappearToLeft:
                case AnimationType.DisappearToRight:
                    page_.SetWidth(1 - phase_ * phase_);
                    break;
            }
            return true;
        }

        public PageAnimation(Page page, AnimationType animationType)
        {
            page_ = page;
            animationType_ = animationType;
            UpdatePage(0);
            switch (animationType)
            {
                case AnimationType.AppearFromLeft:
                case AnimationType.AppearFromRight:
                    phase_ = -0.5f;
                    break;
                default:
                    phase_ = 0;
                    break;
            }
        }
    }
    List<PageAnimation> _pageAnimations;

    public void UpdateButtons()
    {
        ButtonBack.interactable = CurrentPageIndex != 0;
        ButtonForward.interactable = CurrentPageIndex != _pages.Length - 1;
        // Update the counter text. Index 0 means page 1, so add one to the index
        Counter.text = (CurrentPageIndex + 1) + "/" + _pages.Length;
    }

    public void SwitchPage(PageAction pageAction)
    {
        Page previousPage = _pages[CurrentPageIndex];
        Page currentPage;
        switch (pageAction)
        {
            case PageAction.Forward:
                CurrentPageIndex = Mathf.Min(CurrentPageIndex + 1, _pages.Length - 1);
                currentPage = _pages[CurrentPageIndex];
                // Should never happen, but just in case...
                if (currentPage.go == null || currentPage.go == previousPage.go)
                    return;

                _pageAnimations.Add(new PageAnimation (previousPage, AnimationType.DisappearToLeft));
                _pageAnimations.Add(new PageAnimation (currentPage, AnimationType.AppearFromRight));
                break;
            case PageAction.Back:
                CurrentPageIndex = Mathf.Max(CurrentPageIndex - 1, 0);
                currentPage = _pages[CurrentPageIndex];
                // Should never happen, but just in case...
                if (currentPage.go == null || currentPage.go == previousPage.go)
                    return;

                _pageAnimations.Add(new PageAnimation(previousPage, AnimationType.DisappearToRight));
                _pageAnimations.Add(new PageAnimation(currentPage, AnimationType.AppearFromLeft));
                break;
        }
        UpdateButtons();
    }
    private void Start()
    {
        ButtonBack.onClick.AddListener(() => { SwitchPage(PageAction.Back); });
        ButtonForward.onClick.AddListener(() => { SwitchPage(PageAction.Forward); });
    }
    private void OnEnable()
    {
        CurrentPageIndex = 0;

        _pageAnimations = new List<PageAnimation>();
        _pages = new Page[PageObjects.Length];
        for (int i = 0; i < _pages.Length; i++)
        {
            _pages[i] = new Page ( PageObjects[i], i );
            if (i == CurrentPageIndex)
            {
                _pages[i].rt.anchoredPosition = new Vector2((_pages[i].rightBorder + _pages[i].leftBorder) / 2, _pages[i].rt.anchoredPosition.y);
                _pages[i].SetWidth(1);
            }
            else
                _pages[i].SetWidth(0);
        }

        UpdateButtons();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        List<PageAnimation> stoppedAnimations = new List<PageAnimation>();
        foreach (PageAnimation pageAnimation in _pageAnimations)
        {
            if (!pageAnimation.UpdatePage(dt))
            {
                stoppedAnimations.Add(pageAnimation);
            }
        }
        foreach (PageAnimation pageAnimation in stoppedAnimations)
        {
            _pageAnimations.Remove(pageAnimation);
        }
    }
}

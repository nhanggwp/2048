using UnityEngine;
using DG.Tweening;

public class PopUpManager : MonoBehaviour
{
    public GameObject SettingPanelInstance;
    public CanvasGroup OverlayBackGround;
    public bool SwipeLocked = false;

    private void Awake()
    {
        if (OverlayBackGround != null)
        {
            OverlayBackGround.interactable = false;
            OverlayBackGround.alpha = 0f;
        }

        if (SettingPanelInstance != null)
        {
            SettingPanelInstance.SetActive(false);
        }
    }

    public void SettingActivation()
    {
        if (OverlayBackGround != null)
        {
            OverlayBackGround.interactable = true;
            OverlayBackGround.DOFade(1, 0.4f);
        }

        if (SettingPanelInstance != null)
        {
            SettingPanelInstance.SetActive(true);
            SettingPanelInstance.transform.localScale = Vector3.zero;
            SettingPanelInstance.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            SwipeLocked = true;

        }
    }

    public void Exit()
    {
        if (SettingPanelInstance != null)
        {
            SettingPanelInstance.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    SettingPanelInstance.SetActive(false);

                    if (OverlayBackGround != null)
                    {
                        OverlayBackGround.DOFade(0, 0.4f).OnComplete(() => OverlayBackGround.interactable = false);
                    }
                });
        }
        SwipeLocked = false;
    }
}

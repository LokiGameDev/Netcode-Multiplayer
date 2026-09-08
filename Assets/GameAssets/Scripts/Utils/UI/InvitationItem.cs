using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvitationItem : MonoBehaviour
{
    [SerializeField] private TMP_Text friendInvitationText;
    [SerializeField] private FriendsManager friendsManager;
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image timeOutSlider;
    [SerializeField] private float invitationTimeOut = 1;
    [SerializeField] private float slideDuration = 0.5f;

    private Vector2 panelFinalPosition;
    private Coroutine coroutine;

    private string currentInviterName;

    private void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if(panel!=null)
        {
            panel = GetComponent<RectTransform>();
            if(panel!=null) panelFinalPosition = panel.anchoredPosition;
        }
    }

    public void ShowInvitation(string OwnerName, string lobbyName)
    {
        if(coroutine!=null)
        {
            StopCoroutine(coroutine);
        }

        currentInviterName = OwnerName;

        friendInvitationText.text = $"{OwnerName} sent you a invite to join {lobbyName}";

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        float screenWidth = ((RectTransform)panel.parent).rect.width;
        float panelWidth = panel.rect.width;
        
        Vector2 startPos;

        startPos = new Vector2(screenWidth / 2f + panelWidth / 2f,panelFinalPosition.y);
        coroutine = StartCoroutine(DisplayMessageFade(startPos, panel.anchoredPosition));
    }

    private IEnumerator DisplayMessageFade(Vector2 start, Vector2 target)
    {
        AudioManager.Instance.Play(AudioID.Notification);

        float time = 0f;

        while (time < slideDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / slideDuration);

            // Smooth movement
            t = Mathf.SmoothStep(0f, 1f, t);

            panel.anchoredPosition = Vector2.Lerp(start, target, t);

            yield return null;
        }

        panel.anchoredPosition = target;

        yield return new WaitForSeconds(invitationTimeOut/2);

        float elapsed = 0;

        while(elapsed < invitationTimeOut/2)
        {
            elapsed += Time.deltaTime;
            timeOutSlider.fillAmount = Mathf.Lerp(1,0,elapsed/(invitationTimeOut/2));
            yield return null;
        }

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        coroutine = null;
    }

    public void AcceptInvitation()
    {
        friendsManager.AcceptCurrentInvitation(currentInviterName);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        coroutine = null;
    }

    public void CloseInvitation()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        coroutine = null;
    }
}

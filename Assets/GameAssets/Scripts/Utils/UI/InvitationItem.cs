using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays a lobby invitation and its response actions.</summary>
public class InvitationItem : MonoBehaviour
{
    [Header("Invitation")]
    [Tooltip("Text displaying the invitation message.")]
    [SerializeField] private TMP_Text friendInvitationText;
    [Tooltip("Friends manager handling invitation responses.")]
    [SerializeField] private FriendsManager friendsManager;
    [Tooltip("Panel animated when the invitation appears.")]
    [SerializeField] private RectTransform panel;
    [Tooltip("Canvas group used to show and hide the invitation.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [Tooltip("Slider showing the invitation timeout.")]
    [SerializeField] private Image timeOutSlider;
    [Tooltip("Seconds before the invitation expires.")]
    [SerializeField] private float invitationTimeOut = 1;
    [Tooltip("Duration of the invitation slide-in animation.")]
    [SerializeField] private float slideDuration = 0.5f;

    private Vector2 panelFinalPosition;
    private Coroutine coroutine;

    private string currentInviterName;

    /// <summary>Initializes the invitation as hidden.</summary>
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

    /// <summary>Displays an invitation and starts its timeout animation.</summary>
    /// <param name="OwnerName">Name of the inviting player.</param>
    /// <param name="lobbyName">Name of the invited lobby.</param>
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

    /// <summary>Slides in the invitation and fades it after its timeout.</summary>
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

    /// <summary>Accepts the active invitation and hides the prompt.</summary>
    public void AcceptInvitation()
    {
        friendsManager.AcceptCurrentInvitation(currentInviterName);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        coroutine = null;
    }

    /// <summary>Closes the active invitation without joining.</summary>
    public void CloseInvitation()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        coroutine = null;
    }
}

using System.Collections;
using UnityEngine;

/// <summary>Controls player preview interaction in the menu.</summary>
public class MenuPlayerInteraction : MonoBehaviour
{
    [Header("Player Preview")]
    [Tooltip("Player object shown in the menu preview.")]
    [SerializeField] private GameObject playerObject;
    [Tooltip("Animator controlling the preview player.")]
    [SerializeField] private Animator playerAnimator;
    [Tooltip("Touch area used to rotate the preview player.")]
    [SerializeField] private TouchLookArea touchLookArea;
    [Tooltip("Animator controller assigned to the preview.")]
    [SerializeField] private RuntimeAnimatorController animatorController;
    private Coroutine randomActionCoroutine;

    /// <summary>Initializes the preview player and its input callbacks.</summary>
    public void Initialize()
    {
        Debug.Log("Menu player intiated");

        if(playerAnimator==null) playerAnimator = GetComponentInChildren<Animator>();

        if(playerObject==null) playerObject = transform.GetChild(0).gameObject;

        playerAnimator.runtimeAnimatorController = animatorController;

        randomActionCoroutine = StartCoroutine(PlayRandomAction());

        touchLookArea.OnLook += HandlePlayerDrag;
    }

    /// <summary>Stops preview animations when disabled.</summary>
    public void OnDisable()
    {
        StopAllCoroutines();

        randomActionCoroutine = null;
    }

    /// <summary>Stops animations and removes touch callbacks on destruction.</summary>
    public void OnDestroy()
    {
        StopAllCoroutines();

        randomActionCoroutine = null;

        touchLookArea.OnLook -= HandlePlayerDrag;
    }

    /// <summary>Replaces the preview animator with a new skin.</summary>
    /// <param name="currentSkin">Skin object to preview.</param>
    public void ResetSkin(GameObject currentSkin)
    {
        playerAnimator = currentSkin.GetComponent<Animator>();

        playerObject = currentSkin;

        playerAnimator.runtimeAnimatorController = animatorController;
    }

    /// <summary>Plays a random preview animation after a short delay.</summary>
    private IEnumerator PlayRandomAction()
    {
        yield return new WaitForSeconds(Random.Range(5,7));

        playerAnimator.SetInteger("Action", Random.Range(1,4));

        yield return new WaitForSeconds(Random.Range(5,7));

        playerAnimator.SetInteger("Action", 0);

        randomActionCoroutine = null;
    }

    /// <summary>Restarts the idle action routine when it finishes.</summary>
    private void Update()
    {
        if(randomActionCoroutine!=null) return;

        randomActionCoroutine = StartCoroutine(PlayRandomAction());
    }

    /// <summary>Rotates the preview player from touch drag input.</summary>
    /// <param name="drag">Look delta from the touch area.</param>
    private void HandlePlayerDrag(Vector2 drag)
    {
        playerObject.transform.rotation *= Quaternion.Euler(0, -drag.x, 0);
    }
}

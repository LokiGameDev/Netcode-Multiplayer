using UnityEngine;

/// <summary>Restricts device orientation to landscape modes.</summary>
public class LockLandscape : MonoBehaviour
{
    /// <summary>Configures the allowed landscape orientations.</summary>
    private void Awake()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}

using UnityEngine;

/// <summary>Restricts device orientation to landscape modes.</summary>
public class LockLandscape : MonoBehaviour
{
    /// <summary>Configures the allowed landscape orientations.</summary>
    void Awake()
    {
        // Allow only landscape orientations
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        // Force landscape left as default
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    /// <summary>Restores landscape orientation if another mode is active.</summary>
    void Start()
    {
        // Optional: Ensure it stays locked even if device rotates
        if (Screen.orientation != ScreenOrientation.LandscapeLeft &&
            Screen.orientation != ScreenOrientation.LandscapeRight)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }
}

using UnityEngine;

public class LockLandscape : MonoBehaviour
{
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

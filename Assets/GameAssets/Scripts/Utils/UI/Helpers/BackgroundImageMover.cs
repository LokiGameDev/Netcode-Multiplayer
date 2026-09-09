using UnityEngine;
using UnityEngine.UI;

/// <summary>Scrolls or rotates a background image continuously.</summary>
public class BackgroundImageMover : MonoBehaviour
{
    [Header("Background Effect")]
    [Tooltip("Movement or rotation effect to apply.")]
    [SerializeField] private EffectType effectType = EffectType.Movement;
    [Tooltip("Image whose material or transform is animated.")]
    [SerializeField] private Image image;
    [Tooltip("Texture offset change per second.")]
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.1f, 0f);
    [Tooltip("Rotation speed in degrees per second.")]
    [SerializeField] private float rotatingSpeed = 10f;

    private Material material;

    /// <summary>Caches the image material used for texture movement.</summary>
    private void Awake()
    {
        material = image.material;
    }

    /// <summary>Applies the configured background animation each frame.</summary>
    private void Update()
    {
        if(effectType == EffectType.Movement)
        {
            Vector2 offset = material.mainTextureOffset;
            offset += scrollSpeed * Time.deltaTime;

            material.mainTextureOffset = offset;
        }
        else if(effectType == EffectType.Rotate)
        {
            image.rectTransform.Rotate(Vector3.forward, rotatingSpeed * Time.deltaTime);
        }
    }

    /// <summary>Background animation modes.</summary>
    public enum EffectType
    {
        Movement,
        Rotate
    }
}

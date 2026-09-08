using UnityEngine;
using UnityEngine.UI;

public class BackgroundImageMover : MonoBehaviour
{
    [SerializeField] private EffectType effectType = EffectType.Movement;
    [SerializeField] private Image image;
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.1f, 0f);
    [SerializeField] private float rotatingSpeed = 10f;

    private Material material;

    private void Awake()
    {
        material = image.material;
    }

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

    public enum EffectType
    {
        Movement,
        Rotate
    }
}

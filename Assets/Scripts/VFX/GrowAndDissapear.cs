using System.Collections;
using UnityEngine;

public class GrowAndDissapear : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float targetScale = 1.5f;
    [SerializeField] private float targetAlpha = 0.0f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.Constant(0.0f, 1.0f, 1.0f);
    [SerializeField] private bool autoStart = false;
    [SerializeField] private bool destroyOnEnd = true;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (autoStart) Animate();
    }

    public void Animate()
    {
        StartCoroutine(nameof(StartAnimation));
    }

    public void Animate(float duration, float targetScale, float targetAlpha, bool destroyOnEnd)
    {
        this.duration = duration;
        this.targetScale = targetScale;
        this.targetAlpha = targetAlpha;
        this.destroyOnEnd = destroyOnEnd;
        StartCoroutine(nameof(StartAnimation));
    }

    IEnumerator StartAnimation()
    {
        float elapsed = 0.0f;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * targetScale;

        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(startScale, endScale, curve.Evaluate(t));
            spriteRenderer.color = Color.Lerp(startColor, endColor, curve.Evaluate(t));

            elapsed += Time.deltaTime;
            yield return null;
        }


        transform.localScale = endScale;
        spriteRenderer.color = endColor;
        if (destroyOnEnd) Destroy(gameObject);
    }
}

using System.Collections;
using UnityEngine;

public class TelekinesisOverlay : MonoBehaviour
{
    [SerializeField] SpriteRenderer smallOverlayRenderer;
    [SerializeField] SpriteRenderer bigOverlayRenderer;
    [SerializeField] SpriteRenderer overlayRenderer;

    [SerializeField] float appearTime = 0.7f;

    private AnimationCurve curve = new AnimationCurve();

    private void Awake()
    {
        curve.AddKey(0.0f, 0.0f);
        curve.AddKey(0.8f, 1.2f);
        curve.AddKey(1.0f, 1.0f);

        StartOverlayAnimation();
    }

    private void StartOverlayAnimation()
    {
        StartCoroutine(RotateAndAppear(smallOverlayRenderer, 0.0f));
        StartCoroutine(RotateAndAppear(bigOverlayRenderer, 0.1f));
    }

    private IEnumerator RotateAndAppear(SpriteRenderer renderer, float delay)
    {
        Transform overlayTransform = renderer.transform;
        Quaternion startRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        Quaternion endRotation = Quaternion.identity;

        renderer.color = new(.0f, .0f, .0f, .0f);
        overlayTransform.rotation = startRotation;

        yield return new WaitForSeconds(delay);

        float t = 0.0f;

        while (t < appearTime)
        {
            t += Time.deltaTime;

            renderer.color = Color.Lerp(new(.0f, .0f, .0f, .0f), Color.white, t / appearTime);
            overlayTransform.rotation = Quaternion.LerpUnclamped(startRotation, endRotation, curve.Evaluate(t / appearTime));

            yield return null;
        }
    }
}

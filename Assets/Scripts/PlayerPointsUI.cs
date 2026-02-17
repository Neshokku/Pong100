using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerPointsUI : MonoBehaviour
{
    public int score { get; private set; } = 0;
    
    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;

    [Header("Prefabs")]
    [SerializeField] private GameObject indicatorPrefab;

    [Header("Limits")]
    [SerializeField] int maxPoints = 100;
    [SerializeField] int minPoints = 0;

    [Header("Animation")]
    [SerializeField] float shiftTime = 0.5f;
    [SerializeField] float shiftDistance = 60.0f;
    [SerializeField] float stayTime = 0.5f;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void WriteScore()
    {
        textMesh.text = score.ToString();
    }

    public void AddPoints(int pointsToAdd)
    {
        if (pointsToAdd <= 0) return;
        score += pointsToAdd;
        if (score > maxPoints) score = maxPoints;
        WriteScore();
        StartCoroutine(IndicatorRoutine("+" + pointsToAdd));
    }

    public void RemovePoints(int pointsToRemove)
    {
        if (pointsToRemove <= 0) return;
        score -= pointsToRemove;
        if (score < minPoints) score = minPoints;
        WriteScore();
        StartCoroutine(IndicatorRoutine("-" + pointsToRemove));
    }

    private IEnumerator IndicatorRoutine(string text)
    {
        GameObject newIndicator = Instantiate(indicatorPrefab, rectTransform);
        RectTransform newIndicatorRect = newIndicator.GetComponent<RectTransform>();
        TextMeshProUGUI newIndicatorTextMesh = newIndicator.GetComponent<TextMeshProUGUI>();

        if (newIndicator == null || newIndicatorRect == null || newIndicatorTextMesh == null) yield break;

        newIndicatorTextMesh.text = text;

        float t = 0.0f;

        Vector3 initialPosition = newIndicatorRect.anchoredPosition;
        Vector3 finalPosition = new Vector3(
            newIndicatorRect.anchoredPosition.x, 
            newIndicatorRect.anchoredPosition.y - shiftDistance);

        Color initialColor = new Color(
            newIndicatorTextMesh.color.r, 
            newIndicatorTextMesh.color.g, 
            newIndicatorTextMesh.color.b, 
            0.0f);

        Color finalColor = new Color(
            newIndicatorTextMesh.color.r, 
            newIndicatorTextMesh.color.g, 
            newIndicatorTextMesh.color.b, 
            textMesh.color.a);

        while (t < shiftTime)
        {
            t += Time.deltaTime;

            newIndicatorRect.anchoredPosition = Vector3.Lerp(initialPosition, finalPosition, t / shiftTime);
            newIndicatorTextMesh.color = Color.Lerp(initialColor, finalColor, t / shiftTime);
            yield return null;
        }

        newIndicatorRect.anchoredPosition = finalPosition;
        newIndicatorTextMesh.color = finalColor;

        yield return new WaitForSeconds(stayTime);

        t = 0.0f;

        initialPosition = newIndicatorRect.anchoredPosition;
        finalPosition = new Vector3(
            newIndicatorRect.anchoredPosition.x,
            newIndicatorRect.anchoredPosition.y - shiftDistance);

        while (t < shiftTime)
        {
            t += Time.deltaTime;

            newIndicatorRect.anchoredPosition = Vector3.Lerp(initialPosition, finalPosition, t / shiftTime);
            newIndicatorTextMesh.color = Color.Lerp(finalColor, initialColor, t / shiftTime);
            yield return null;
        }

        Destroy(newIndicator);
    }
}

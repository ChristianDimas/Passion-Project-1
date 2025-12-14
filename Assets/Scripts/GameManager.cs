using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private ChapterSO chapter;

    [Header("Page Animation Settings")]
    [SerializeField] private AnimationCurve smoothGraph;
    [SerializeField] private float pageFadeInOutDuration = .5f;
    [SerializeField] private float typeSpeed = 0.01f;

    public bool isPlaying = false;
    public Action togglePlayPause;

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera vcam1;
    [SerializeField] private CinemachineCamera vcam2;
    private float shotDuration = 1f;
    public bool isTalking = false;


    [Header("Prefabs")]
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private GameObject speechBubbleCanvasPrefab;
    [SerializeField] private GameObject speechBubblePrefab;

    [Header("Curtains and Masks")]
    [SerializeField] private GameObject fadeCurtain;
    [SerializeField] private GameObject partialFadeCurtain;
    [SerializeField] private GameObject shotMask;

    SpriteRenderer faceCurtainSr => fadeCurtain.GetComponent<SpriteRenderer>();
    SpriteRenderer partialFadeCurtainSr => partialFadeCurtain.GetComponent<SpriteRenderer>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator AnimateChapter()
    {
        bool usingVcam1 = true;

        foreach (var page in chapter.pages)
        {
            // Page Setup
            GameObject currentPage = Instantiate(pagePrefab, Vector3.zero, Quaternion.identity);
            GameObject speechBubbleCanvas = null;
            SpriteRenderer sr = currentPage.GetComponent<SpriteRenderer>();
            sr.sprite = page.pageImage;

            StartCoroutine(FadePage(1f, 0f, pageFadeInOutDuration, smoothGraph));
            StartCoroutine(TogglePartialFadeCurtain(1f, smoothGraph, true));

            // yield return new WaitForSeconds(pageFadeInDuration + 0.5f);

            List<Shot> shots = page.shots;

            foreach (var shot in shots)
            {
                float ortoRatio = shot.shotScale.y / 10.67f; // Prioritize y zoom first

                if (ortoRatio < shot.shotScale.x / 19.2f)
                {
                    ortoRatio = shot.shotScale.x / 19.2f;
                }
                if (usingVcam1)
                {
                    vcam2.transform.position = new Vector3(shot.shotPosition.x, shot.shotPosition.y, -0.3f); // Set Z to -0.3f to avoid clipping
                    vcam2.Lens.OrthographicSize = ortoRatio * 5.4f;
                    vcam1.Priority = 0;
                    vcam2.Priority = 10;
                    usingVcam1 = false;
                }
                else
                {
                    vcam1.transform.position = new Vector3(shot.shotPosition.x, shot.shotPosition.y, -0.3f);
                    vcam1.Lens.OrthographicSize = ortoRatio * 5.4f;
                    vcam1.Priority = 10;
                    vcam2.Priority = 0;
                    usingVcam1 = true;
                }

                StartCoroutine(MoveMask(shot, 1f, smoothGraph)); // Remember to adjust duration based on Cinemachine blend time

                if (shot.speechBubbles.Count > 0)
                {
                    if (speechBubbleCanvas == null)
                    {
                        speechBubbleCanvas = Instantiate(speechBubbleCanvasPrefab, currentPage.transform);
                    }

                    yield return new WaitForSeconds(.7f); // Wait for camera to settle

                    foreach (var bubble in shot.speechBubbles)
                    {
                        GameObject speechBubble = Instantiate(speechBubblePrefab, speechBubbleCanvas.transform);

                        RectTransform rt = speechBubble.GetComponent<RectTransform>();
                        rt.anchoredPosition = bubble.sbPosition;
                        rt.sizeDelta = bubble.sbWidthHeight;
                        rt.localScale = Vector3.one;

                        TMPro.TextMeshProUGUI tmp = speechBubble.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                        tmp.fontSize = bubble.sbFontSize;
                        tmp.text = bubble.sbText;
                        float sbDuration = bubble.sbText.ToCharArray().Count() * typeSpeed;
                        tmp.text = string.Empty;
                        StartCoroutine(TypeLine(tmp, bubble.sbText));
                        AudioManager.Instance.StartTypingSFX();

                        yield return new WaitForSeconds(sbDuration + 0.5f);

                        AudioManager.Instance.StopTypingSFX();

                    }
                    yield return new WaitForSeconds(shotDuration);

                }
                else
                {
                    yield return new WaitForSeconds(shotDuration + 1); // Extra second to appreciate shot without speech bubbles
                }
            }
            StartCoroutine(FadePage(0f, 1f, pageFadeInOutDuration, smoothGraph));
            StartCoroutine(TogglePartialFadeCurtain(1f, smoothGraph, false));

            yield return new WaitForSeconds(pageFadeInOutDuration);
            Destroy(currentPage);
        }
        yield return null;
    }

    IEnumerator MoveMask(Shot shot, float duration, AnimationCurve curve)
    {
        float t = 0f;
        Vector3 startPos = shotMask.transform.position;
        Vector3 endPos = new Vector3(shot.shotPosition.x, shot.shotPosition.y, startPos.z);
        Vector3 startScale = shotMask.transform.localScale;
        Vector3 endScale = new Vector3(shot.shotScale.x, shot.shotScale.y, startScale.z);

        while (t < duration)
        {
            t += Time.deltaTime;

            float curved = curve.Evaluate(t / duration);

            shotMask.transform.position = Vector3.Lerp(startPos, endPos, curved);
            shotMask.transform.localScale = Vector3.Lerp(startScale, endScale, curved);

            yield return null;
        }

        shotMask.transform.position = endPos;
        shotMask.transform.localScale = endScale;
    }


    IEnumerator TogglePartialFadeCurtain(float duration, AnimationCurve curve, bool activate)
    {
        float t = 0f;
        Color c = partialFadeCurtainSr.color;
        float startAlpha = activate ? 0f : .6f;
        float endAlpha = activate ? .6f : 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float curved = curve.Evaluate(t / duration);

            c.a = Mathf.Lerp(startAlpha, endAlpha, curved);
            partialFadeCurtainSr.color = c;

            yield return null;
        }

        c.a = endAlpha;
        partialFadeCurtainSr.color = c;
    }

    IEnumerator FadePage(float fromAlpha, float toAlpha, float duration, AnimationCurve curve)
    {
        float t = 0f;
        Color c = faceCurtainSr.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float curved = curve.Evaluate(t / duration);

            c.a = Mathf.Lerp(fromAlpha, toAlpha, curved);
            faceCurtainSr.color = c;

            yield return null;
        }

        c.a = toAlpha;
        faceCurtainSr.color = c;
    }


    IEnumerator TypeLine(TMPro.TextMeshProUGUI tmp, string line)
    {
        foreach (char c in line.ToCharArray())
        {
            tmp.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    public void TogglePlayPause()
    {
        if (!isPlaying)
        {
            StartCoroutine(AnimateChapter());
            togglePlayPause?.Invoke();
        }
    }
}

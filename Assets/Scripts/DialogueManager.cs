using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [SerializeField] private ConversationSO conversation;

    public bool isPlaying = false;
    public Action togglePlayPause;


    [Header("Dialogue Animation Settings")]
    [SerializeField] private AnimationCurve smoothGraph;
    [SerializeField] private float dialogueFadeDuration = .5f;
    [SerializeField] private float typeSpeed = 0.01f;
    [SerializeField] private float waituntilNextLineDuration = 1f;

    [Header("Dialogue Components")]
    [SerializeField] private CanvasGroup dialogueBox;
    [SerializeField] private Image portraitImg;
    [SerializeField] private TMPro.TextMeshProUGUI characterNameText;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;

    void Awake()
    {
        Instance = this;
        dialogueBox.alpha = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }


    IEnumerator StartConversation()
    {
        for (int i = 0; i < conversation.lines.Count; i++)
        {
            Line line = conversation.lines[i];
            CharacterSO character = line.character;

            portraitImg.sprite = character.characterSprite[line.characterSpriteIndex];
            characterNameText.text = character.characterName.ToString();
            dialogueText.text = string.Empty;

            bool needsFadeIn =
                i == 0 ||
                conversation.lines[i - 1].character != line.character ||
                conversation.lines[i - 1].characterSpriteIndex != line.characterSpriteIndex;

            if (needsFadeIn)
            {
                yield return FadeDialogueBox(0f, 1f, dialogueFadeDuration, smoothGraph);
            }

            yield return TypeLine(dialogueText, line.dialogueText);
            yield return new WaitForSeconds(waituntilNextLineDuration);

            bool needsFadeOut =
                i < conversation.lines.Count - 1 &&
                (conversation.lines[i + 1].character != line.character ||
                 conversation.lines[i + 1].characterSpriteIndex != line.characterSpriteIndex);

            if (needsFadeOut)
            {
                yield return FadeDialogueBox(1f, 0f, dialogueFadeDuration, smoothGraph);
            }
        }
    }

    IEnumerator FadeDialogueBox(float fromAlpha, float toAlpha, float duration, AnimationCurve curve)
    {
        float t = 0f;


        while (t < duration)
        {
            t += Time.deltaTime;
            float curved = curve.Evaluate(t / duration);
            dialogueBox.alpha = Mathf.Lerp(fromAlpha, toAlpha, curved);

            yield return null;
        }

        dialogueBox.alpha = toAlpha;
    }

    IEnumerator TypeLine(TMPro.TextMeshProUGUI tmp, string text)
    {
        foreach (char c in text.ToCharArray())
        {
            tmp.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    public void TogglePlayPause()
    {
        if (!isPlaying)
        {
            StartCoroutine(StartConversation());
            togglePlayPause?.Invoke();
        }
    }
}

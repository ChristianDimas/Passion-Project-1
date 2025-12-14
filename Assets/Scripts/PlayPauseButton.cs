using UnityEngine;
using UnityEngine.UI;

public class PlayPauseButton : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private Image image;
    private int currentSpriteIndex = 0;

    void Start()
    {
        image = GetComponent<Image>();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.togglePlayPause += ChangeSprite;
        }
        else if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.togglePlayPause += ChangeSprite;
        }

    }

    private void ChangeSprite()
    {
        if (currentSpriteIndex == 0)
        {
            currentSpriteIndex = 1;
            image.sprite = sprites[currentSpriteIndex];
        }
        else
        {
            currentSpriteIndex = 0;
            image.sprite = sprites[currentSpriteIndex];
        }
    }
}

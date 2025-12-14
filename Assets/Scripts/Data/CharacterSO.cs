using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    public Character characterName;
    public Sprite[] characterSprite;
}

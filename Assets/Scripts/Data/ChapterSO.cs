using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chapter", menuName = "ScriptableObjects/ChapterSO")]
public class ChapterSO : ScriptableObject
{
    public int chapterNumber;
    public List<PageSO> pages;
}

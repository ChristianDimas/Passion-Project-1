using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Page", menuName = "ScriptableObjects/PageSO")]
public class PageSO : ScriptableObject
{
    public Sprite pageImage;
    public List<Shot> shots;
}

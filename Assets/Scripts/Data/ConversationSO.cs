using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation", menuName = "ScriptableObjects/ConversationSO")]
public class ConversationSO : ScriptableObject
{
    public List<Line> lines;
}

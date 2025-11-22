using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Controller/InteractionList")]
public class InteractListById : ScriptableObject
{
    public List<InteractEntry> entries;

    public InteractActionData GetInteraction(string hitAreaId, CursorData cursor)
    {
        foreach (var e in entries)
        {
            if (e.hitAreaId == hitAreaId && e.cursor == cursor)
                return e.action;
        }
        return null;
    }
}

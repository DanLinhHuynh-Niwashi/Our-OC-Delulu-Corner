using UnityEngine;
using UnityEngine.LightTransport;

public struct InteractionContext
{
    public CharacterModel Model;
    public InteractionController Controller;
    public InteractActionData Data;
}

public abstract class InteractionLogic : ScriptableObject
{
    public abstract void Execute(InteractionContext ctx);
    public abstract void End(InteractionContext ctx);

    public virtual bool IsBusy(InteractionContext ctx) { return false; }
}

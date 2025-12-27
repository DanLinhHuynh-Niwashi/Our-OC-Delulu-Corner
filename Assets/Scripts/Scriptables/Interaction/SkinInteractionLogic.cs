using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Logic/Skin")]
public class SkinInteractionLogic : InteractionLogic
{
    public SkinData skinData;

    public override void Execute(InteractionContext ctx)
    {
        if (skinData == null)
            return;

        ctx.Model.skinController.SetSkin(skinData);
    }

    public override void End(InteractionContext ctx)
    {
        return;
    }
}

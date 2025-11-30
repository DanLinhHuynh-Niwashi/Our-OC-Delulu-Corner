using UnityEngine;
using Yarn.Unity;

public class DialogController : MonoBehaviour
{
    public DialogueRunner runner;
    public CharacterModel cubismModel;
    public string nodeToStart = "HitNode";

    public void Init(CharacterModel model)
    {
        cubismModel = model;
    }

    public void OnInteraction(InteractActionData data)
    {
        if (data.targetDialog != "")
        {
            HaltDialog();
            RunDialog(data.targetDialog);
        }
    }
    public void RunDialog(string nodeName)
    {
        if (!runner.IsDialogueRunning)
        {
            runner.StartDialogue(nodeToStart);
        }
        else Debug.Log("Dialogue already running");
    }

    public bool IsBusy()
    {
        return runner.IsDialogueRunning;
    }

    public void HaltDialog()
    {
        runner.Stop();
    }    
}

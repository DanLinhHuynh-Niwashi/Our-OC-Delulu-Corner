
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Markup;
using Yarn.Unity.Attributes;
using TMPro;

#nullable enable

namespace Yarn.Unity
{
    public class CustomLineButtonHandler : ActionMarkupHandler
    {
        [MustNotBeNull, SerializeField] Button? continueButton;

        [MustNotBeNullWhen(nameof(continueButton), "A " + nameof(DialogueRunner) + " must be provided for the continue button to work.")]
        [SerializeField] DialogueRunner? dialogueRunner;

        void Awake()
        {
            if (continueButton == null)
            {
                Debug.LogWarning($"The {nameof(continueButton)} is null, is it not connected in the inspector?", this);
                return;
            }
            continueButton.interactable = false;
            continueButton.enabled = false;
        }

        public override void OnPrepareForLine(MarkupParseResult line, TMP_Text text)
        {
            Debug.Log("DisplayPrepare");
            if (continueButton == null)
            {
                Debug.LogWarning($"The {nameof(continueButton)} is null, is it not connected in the inspector?", this);
                return;
            }
            // enable the button
            continueButton.interactable = true;
            continueButton.enabled = true;

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                if (dialogueRunner == null)
                {
                    Debug.LogWarning($"Continue button was clicked, but {nameof(dialogueRunner)} is null!", this);
                    return;
                }

                Debug.Log("Hurry");
                dialogueRunner.RequestHurryUpLine();
            });
        }

        public override void OnLineDisplayBegin(MarkupParseResult line, TMP_Text text)
        {
            return;
        }

        public override YarnTask OnCharacterWillAppear(int currentCharacterIndex, MarkupParseResult line, CancellationToken cancellationToken)
        {
            return YarnTask.CompletedTask;
        }

        public override void OnLineDisplayComplete()
        {
            Debug.Log("DisplayComplete");
            if (continueButton == null)
            {
                Debug.LogWarning($"The {nameof(continueButton)} is null, is it not connected in the inspector?", this);
                return;
            }
            // enable the button
            continueButton.interactable = true;
            continueButton.enabled = true;

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                if (dialogueRunner == null)
                {
                    Debug.LogWarning($"Continue button was clicked, but {nameof(dialogueRunner)} is null!", this);
                    return;
                }

                Debug.Log("Skip");
                dialogueRunner.RequestNextLine();
            });
        }

        public override void OnLineWillDismiss()
        {
            if (continueButton == null)
            {
                return;
            }
            // disable interaction
            continueButton.onClick.RemoveAllListeners();
            continueButton.interactable = false;
            continueButton.enabled = false;
        }
    }
}

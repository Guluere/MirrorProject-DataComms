using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using MyBox;

/*!!!Notes: 
 * Script Version (1.3V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * 
 * Update Log:
 * 1.0V 27/10/2022: - Basic introduction with a corutine for char to char loading
 * 1.1V 31/10/2022: - Includes a char to char, word to word, etc, calling as well as their waits and text structures
 * 1.2V 02/11/2022: - Create a way to properly chain dialogs together for a simple forward and backward changes.
 * 1.3V 06/11/2022: - Added a way to add choices into the system
 * 
 * 1.4V 10/03/2023: - Overhaul the dialogs, now uses ScriptableObject to log/write the texts
 *                  - Renamed SimpleTextFlow -> DialogPrinter
 */

public class DialogPrinter : MonoBehaviour
{
    [System.Serializable]
    public class ButtonChoiceWithText
    {
        public Button Choice;
        public TextMeshProUGUI ChoiceText;
    }
    /*
    [System.Serializable]
    private class TextListPoint
    {
        [TextArea]
        public string PointText;

        [MinValue(0)]
        public float CharByCharDelay;
        [MinValue(0)]
        public float WordByWordDelay;
        [MinValue(0)]
        public float SentenceDelay;

        public WaitForSeconds CharByCharWait;
        public WaitForSeconds WordByWordWait;
        public WaitForSeconds SentenceWait;
    }

    [System.Serializable]
    private class TextListChain
    {
        [SerializeField]
        public List<TextListPoint> TextListPoints;
    }
    */


    [SerializeField]
    private TextMeshProUGUI TargetTalkerName;

    [SerializeField]
    private TextMeshProUGUI TargetTextMesh;

    [SerializeField]
    private List<UnityEvent> eventListing;

    //[SerializeField]
    //private List<TextListChain> TextListChains;

    public Dialogs LoadedDialog;

    [Min(0f)]
    public float CurrentWaitMultiplier;

    public Dictionary<char, WaitForSeconds> WaitSecondsDictionary = new Dictionary<char, WaitForSeconds>();
    private WaitForSeconds DefaultWaitDelay;

    public int CurrentID { get; private set; }
    public Coroutine CurrentActiveChain { get; private set; }

    [SerializeField]
    private List<ButtonChoiceWithText> Choices;

    private void Start()
    {
        ResetDialog();
        Initialize();
    }

    private void Initialize()
    {
        for(int i = 0; i < Choices.Count; i++)
        {
            var _i = i;
            Choices[i].Choice.onClick.AddListener(() => ButtonChoice(_i));
        }
        TurnOffButtons();
    }

    public void ResetDialog()
    {
        UpdateWaits(CurrentWaitMultiplier); //Todo, add away to speed up or slow down waits
        CurrentID = -1;
        BreakTextChain();
    }

    public void TriggerEventListing(int ID)
    {
        if (ID >= eventListing.Count) return;

        eventListing[ID].Invoke();
    }

    public void TurnOffButtons()
    {
        foreach (ButtonChoiceWithText buttonChoiceWithText in Choices)
        {
            buttonChoiceWithText.Choice.gameObject.SetActive(false);
        }
    }

    public void TurnOnSelectedButton(int ID)
    {
        Choices[ID].Choice.gameObject.SetActive(true);
    }

    public void TurnOnExpectedButton()
    {
        for (int i = 0; i < Mathf.Min(LoadedDialog.Talking[CurrentID].OptionTexts.Length, Choices.Count); i++)
        {
            Choices[i].ChoiceText.text = LoadedDialog.Talking[CurrentID].OptionTexts[i].ChoiceText;
            TurnOnSelectedButton(i);
        }
    }

    public void UpdateWaits(float ValueMultiplier)
    {
        WaitSecondsDictionary.Clear();

        ValueMultiplier = Mathf.Max(ValueMultiplier, 0); //NegativeValues are not allowed

        foreach (Dialogs.TalkingPoint.DelayWithChar delayWithChar in LoadedDialog.Talking[CurrentID].Delays)
        {
            if (!WaitSecondsDictionary.ContainsKey(delayWithChar.Char))
            {
                WaitSecondsDictionary.Add(delayWithChar.Char, new WaitForSeconds(delayWithChar.Delay * ValueMultiplier));
            }
            else WaitSecondsDictionary[delayWithChar.Char] = new WaitForSeconds(delayWithChar.Delay * ValueMultiplier);
        }
    }

    public Coroutine BeginTextChain(int ChainID)
    {
        CurrentID = ChainID;
        UpdateWaits(CurrentWaitMultiplier); //Todo, add away to speed up or slow down waits
        CurrentActiveChain = StartCoroutine("BeginBuildChain", ChainID);
        return CurrentActiveChain;
    }

    private IEnumerator BeginBuildChain(int ChainID)
    {
        string CurrentText = "";
        CurrentID = ChainID;

        TurnOffButtons(); //Turn off options
        TriggerEventListing(LoadedDialog.Talking[ChainID].OnStartTriggerEventID);

        TargetTalkerName.text = LoadedDialog.Talking[CurrentID].TalkerName;

        foreach (char character in LoadedDialog.Talking[CurrentID].TalkingText)
        {
            CurrentText = CurrentText + character;
            TargetTextMesh.text = CurrentText;
            Debug.Log(CurrentText);
            if (WaitSecondsDictionary.ContainsKey(character))
            {
                yield return WaitSecondsDictionary[character];
            }
            else
            {
                TriggerEventListing(LoadedDialog.Talking[ChainID].OnEndTriggerEventID);
                yield return DefaultWaitDelay;
            }
        }
        StopCoroutine(CurrentActiveChain);
        CurrentActiveChain = null;
        TriggerEventListing(LoadedDialog.Talking[ChainID].OnEndTriggerEventID);
        TurnOnExpectedButton(); //Turn on Expected options
    }

    public void BreakTextChain() 
    {
        if (CurrentActiveChain != null) StopCoroutine(CurrentActiveChain);
        CurrentActiveChain = null;
        TargetTextMesh.text = "";
        TurnOffButtons();
    }

    public void InstantFinish()
    {
        StopCoroutine(CurrentActiveChain);
        CurrentActiveChain = null;
        
        TargetTextMesh.text = LoadedDialog.Talking[CurrentID].TalkingText;
    }

    public void ButtonChoice(int Choice)
    {
        if(CurrentActiveChain != null)
        {
            InstantFinish();
            return;
        }
        BeginTextChain(LoadedDialog.Talking[CurrentID].OptionTexts[Choice].LoadTalkingPointPosition);
    }

    public void AdvanceForward()
    {
        if (CurrentActiveChain == null)
        {
            BeginTextChain(++CurrentID);
            return;
        }
        InstantFinish();
    }
    public void AdvanceBackward()
    {
        if (CurrentActiveChain == null)
        {
            BeginTextChain(--CurrentID);
            return;
        }
        InstantFinish();
    }

    [ButtonMethod]
    public void BeginTest()
    {
        BeginTextChain(0);
    }
    [ButtonMethod]
    public void Button0()
    {
        ButtonChoice(0);
    }
    [ButtonMethod]
    public void IncreaseSpeedTest()
    {
        UpdateWaits(0.5f);
    }
}

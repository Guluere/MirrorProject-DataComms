using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using MyBox;

/*!!!Notes: 
 * Script Version (1.0V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * 
 * Update Log:
 * 1.0V 09/03/2023: - Simple introduction giving the ability to set position, and texts, person name, and choices texts.
 */


[CreateAssetMenu(fileName = "Dialogs", menuName = "ScriptableObjects/Dialogs", order = 1)]
public class Dialogs : ScriptableObject
{
    public enum Position
    {
        Left,
        Right
    }

    //public string Name; //Name of Dialog, a component will look though all dialogs just for this current dialog.
    public TalkingPoint[] Talking;

    [System.Serializable]
    public class TalkingPoint
    {
        public Position TalkingPosition;
        [TextArea]
        public string TalkerName;
        [TextArea]
        public string TalkingText;
        public Optional[] OptionTexts;

        public DelayWithChar[] Delays;
        [MinValue(0)]
        public float DefaultDelay; //If the char is not in the delays, this delay will get used.

        public int OnStartTriggerEventID;

        public int DefaultCharTriggerEventID;

        public int OnEndTriggerEventID;

        [System.Serializable]
        public class DelayWithChar
        {
            public char Char;
            [MinValue(0)]
            public float Delay;

            public int TriggerEventID;
        }

        [System.Serializable]
        public class Optional
        {
            [TextArea]
            public string ChoiceText;
            [MinValue(0)]
            public int LoadTalkingPointPosition; //Within this current Dialog, this choice will jump towards that text.

            public int TriggerEventID;
        }
    }
}

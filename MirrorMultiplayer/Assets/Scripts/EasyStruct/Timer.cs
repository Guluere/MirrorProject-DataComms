using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

/*!!!Notes: 
 * Script Version (1.2V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * 
 * Update Log:
 * 1.0V     -     : - Introduced with a basic on collision involved event calling, as well as conditional value increasing and decrease before potential calls.
 * 1.1V     -     : - Removed conditional value calling and made it a seperate component.
 * 1.2V     -     : - Included trigger only event calls, as well as the ability to specify what collision layer should be targeted seperately.
 */

public class Timer : MonoBehaviour
{
    [System.Serializable]
    public class TimerToEvent
    {
        [Min(0)]
        public float Time;
        public UnityEvent Event;
    }
    [SerializeField]
    public TimerToEvent[] EventList;

    public void StartTimer(int EventID)
    {
        StartCoroutine("CoTimer", EventID);
    }

    private IEnumerator CoTimer(int EventID)
    {
        yield return new WaitForSeconds(EventList[EventID].Time);
        EventList[EventID].Event.Invoke();
    }
}

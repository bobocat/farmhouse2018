//          ======= Copyright (c) BlackPandaStudios, All rights reserved. =======
//              _____________________________________________________________ 
// <author>    | -  Joel Gabriel                                             |  
// <date>      | -  28/03/2017                                               |  
// <name>      | -  Helper.cs                                                |  
// <summary>   | -  This class is simply a wrapper for useful unity helper   |            
//             |    functions that came in handy when developing this plugin.|      
//             |_____________________________________________________________|

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Loco.Helpers {
    public class Helper : MonoBehaviour {
        /// <summary>
        /// Returns true when the timer is greater than the maxTime.
        /// </summary>
        /// <param name="a_timer">The value that is increasing/timer.</param>
        /// <param name="a_maxTime">What you are comparing the timer value to.</param>
        /// <returns></returns>
        public static bool TimerElapsed(float a_timer, float a_maxTime){
            if (a_timer >= a_maxTime) return true;
            else return false;
        }

        /// <summary>
        /// Returns true while the timer is running.
        /// </summary>
        /// <param name="a_timer">The value that is increasing/timer.</param>
        /// <param name="a_maxTime">What you are comparing the timer value to.</param>
        /// <returns></returns>
        public static bool TimerRunning(float a_timer, float a_maxTime){
            if (a_timer < a_maxTime) return true;
            else return false;
        }

        /// <summary>
        /// Destroys a GameObject after 'X' amount of seconds.
        /// </summary>
        /// <param name="a_toDestroy">The GameObject you wish to destroy.</param>
        /// <param name="a_timeToDestroy">How long to wait, until you destroy the GameObject.</param>
        /// <returns></returns>
        public static IEnumerator DestroyOverTime(GameObject a_toDestroy, float a_timeToDestroy){
            // simple destroys a gameobject after a set time
            yield return new WaitForSeconds(a_timeToDestroy);
            Destroy(a_toDestroy);
        }

        /// <summary>
        /// A function that will Invoke a method after a 'X' amount of seconds.
        /// </summary>
        /// <param name="a_event">This holds the event/function you want to Invoke.</param>
        /// <param name="a_timeToExecute">This is the time you wait, before executing this function/event.</param>
        /// <returns></returns>
        public static IEnumerator ExecuteFunctionAfterTime(UnityEvent a_event, float a_timeToExecute)
        {
            // simply executes a single function after a set time.
            yield return new WaitForSeconds(a_timeToExecute);
            a_event.Invoke();
        }

        /// <summary>
        /// Executes consecutive functions with a delay between each.
        /// </summary>
        /// <param name="a_events">A list of events that will be called one after another.</param>
        /// <param name="a_timeToExecute">This is the time you wait, between executing each function/event.</param>
        /// <returns></returns>
        public static IEnumerator ExecuteConsecutiveFunctions(List<UnityEvent> a_events, float a_timeToExecute)
        {
            // Ensure you have some events, if not wait a frame and check again
            if (a_events.Count == 0) yield return null;
            // for each event in the list, wait the set time, and then invoke the method
            foreach(UnityEvent e in a_events){
                yield return new WaitForSeconds(a_timeToExecute);
                e.Invoke();
            }
        }

        /// <summary>
        /// Executes consequtive functions, with a unique delay between each.
        /// </summary>
        /// <param name="a_events">A list of events that will be called one after another.</param>
        /// <param name="a_timesToExecute">A list of float delays that will be called used one after another. This way you can set the first float in the list to be 0, making it execute instanly, while the rest are timed sequentially</param>
        /// <returns></returns>
        public static IEnumerator ExecuteConsecutiveFunctions(List<UnityEvent> a_events, List<float> a_timesToExecute)
        {
            // checking both lists have data, if not, wait a frame.
            if (a_events.Count == 0 || a_timesToExecute.Count == 0) yield return null;

            // checks to see if you have less events then you do times,
            // if you do that isnt good! log a warning explaining the issue, wait a frame!
            if (a_events.Count < a_timesToExecute.Count) {
                Debug.LogWarning("WARNING: you have more a_timesToExecute in your list then you do in you List of events!/n make sure your list of execute times are equal or less than the count of you event list!");
                yield return null;
            // otherwise, checks to see if your event count is greater than or equal to the times count
            // if so, that can be handled by calculating the difference in count, and filling the remainder of the times list with the very last value in the list
            // so for example, if you have 11 events but only 5 time delays, it will do the unique time delays sequentially, and for the 5th value, simply reuse that for the rest.
            } else if (a_events.Count >= a_timesToExecute.Count) {
                int difference = a_events.Count - a_timesToExecute.Count;
                float lastValue = a_timesToExecute[a_timesToExecute.Count - 1];
                for (int i = 0; i < difference; i++) {
                    a_timesToExecute.Add(lastValue);
                }
            }
            
            // After it passes all the above requirements, if will simply wait the delay at that specific index,
            // and then invoke the method at that index
            for(int i = 0; i < a_events.Count; i++){
                yield return new WaitForSeconds(a_timesToExecute[i]);
                a_events[i].Invoke();
            }
        }
    }
}

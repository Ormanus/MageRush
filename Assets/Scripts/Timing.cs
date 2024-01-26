using System.Collections;
using UnityEngine;

namespace Outloud.Common
{
    public static class Timing
    {
        public delegate void TimingDelegate();
        private class CoroutineTimer : MonoBehaviour
        {
            IEnumerator CoroutinePerformer(TimingDelegate timingDelegate, float seconds, bool realTime)
            {
                if (realTime)
                    yield return new WaitForSecondsRealtime(seconds);
                else
                    yield return new WaitForSeconds(seconds);

                timingDelegate.Invoke();
                Destroy(gameObject);
            }

            public void TimeCoroutine(TimingDelegate timingDelegate, float seconds, bool realTime)
            {
                StartCoroutine(CoroutinePerformer(timingDelegate, seconds, realTime));
            }
        }

        public static void DoAfter(TimingDelegate timingDelegate, float seconds, bool realTime = true)
        {
            GameObject obj = new GameObject("Coroutine Timer");
            CoroutineTimer timer = obj.AddComponent<CoroutineTimer>();
            timer.TimeCoroutine(timingDelegate, seconds, realTime);
        }
    }

}

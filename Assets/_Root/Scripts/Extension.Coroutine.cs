namespace Pancake.Common
{
    public static class LanceRoutine
    {
        /// <summary>
        /// Waits for the specified amount of frames
        /// use : yield return LanceRoutine.WaitForFrames(1);
        /// </summary>
        /// <param name="frameCount"></param>
        /// <returns></returns>
        public static System.Collections.IEnumerator WaitForFrames(int frameCount)
        {
            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }

        /// <summary>
        /// Waits for the specified amount of seconds (using regular time)
        /// use : yield return LanceRoutine.WaitFor(1f);
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static System.Collections.IEnumerator WaitFor(float seconds)
        {
            for (var timer = 0f; timer < seconds; timer += UnityEngine.Time.deltaTime)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Waits for the specified amount of seconds (using unscaled time)
        /// use : yield return LanceRoutine.WaitForUnscaled(1f);
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static System.Collections.IEnumerator WaitForUnscaled(float seconds)
        {
            for (var timer = 0f; timer < seconds; timer += UnityEngine.Time.unscaledDeltaTime)
            {
                yield return null;
            }
        }
    }
}
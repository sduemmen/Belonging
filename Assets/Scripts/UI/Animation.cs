using LeanTween.Framework;
using UnityEngine;

namespace UI
{
    public static class Animation
    {
        public static void Cancel(int id)
        {
            LeanTween.Framework.LeanTween.cancel(id);
        }

        public static LTDescr Scale(GameObject obj, Vector3 scale, float time, LeanTweenType curve)
        {
            return LeanTween.Framework.LeanTween.scale(obj, scale, time).setEase(curve);
        }

        public static LTDescr Scale(Transform transform, Vector3 scale, float time, LeanTweenType curve)
        {
            return Scale(transform.gameObject, scale, time, curve);
        }

        public static float CalculateAnimationTime(float animationDuration, LTDescr runningAnimation)
        {
            float time;

            if (runningAnimation == null)
            {
                time = animationDuration;
            }
            else
            {
                time = runningAnimation.passed;
                if (time == 0) time = animationDuration;
            }

            return time;
        }
    }
}
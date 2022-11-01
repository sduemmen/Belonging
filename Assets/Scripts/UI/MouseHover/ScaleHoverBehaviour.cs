using System;
using LeanTween.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.MouseHover
{
    [Serializable]
    [CreateAssetMenu(menuName = "Behaviours/UI/Hover/Scale")]
    public class ScaleHoverBehaviour : ScriptableObject, IMouseHoverBehaviour
    {
        [Title("Settings")] [SerializeField] [Space(5)]
        private Vector3 scaleFactor = Vector3.one;

        [SerializeField] [Space(5)] private bool enableAnimation;

        [SerializeField] [TitleGroup("Animation Settings")] [ShowIf("enableAnimation")]
        private float animationDuration;

        [SerializeField] [TitleGroup("Animation Settings")] [ShowIf("enableAnimation")]
        private LeanTweenType hoverEnterAnimationCurve;

        [SerializeField] [TitleGroup("Animation Settings")] [ShowIf("enableAnimation")]
        private LeanTweenType hoverLeaveAnimationCurve;

        public void OnHoverEnter(Hoverable hoverable)
        {
            if (enableAnimation)
            {
                LTDescr currentAnimation = LeanTween.Framework.LeanTween.descr(hoverable.CurrentAnimationID);
                
                float time = Animation.CalculateAnimationTime(animationDuration, currentAnimation);
                Animation.Cancel(hoverable.CurrentAnimationID);
                hoverable.CurrentAnimationID = Animation.Scale(hoverable.transform, scaleFactor, time, hoverEnterAnimationCurve).id;
            }
            else
            {
                hoverable.transform.localScale = scaleFactor;
            }
        }

        public void OnHoverLeave(Hoverable hoverable)
        {
            if (enableAnimation)
            {
                LTDescr currentAnimation = LeanTween.Framework.LeanTween.descr(hoverable.CurrentAnimationID);
                
                float time = Animation.CalculateAnimationTime(animationDuration, currentAnimation);
                Animation.Cancel(hoverable.CurrentAnimationID);
                hoverable.CurrentAnimationID = Animation.Scale(hoverable.transform, hoverable.OriginalScale, time, hoverLeaveAnimationCurve).id;
            }
            else
            {
                hoverable.transform.localScale = hoverable.OriginalScale;
            }
        }
    }
}
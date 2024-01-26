using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Outloud.Common
{
    [RequireComponent(typeof(RectTransform))]
    public class UISlideTransition : MonoBehaviour
    {
        public enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }

        public enum State
        {
            In,
            Out,
            MovingIn,
            MovingOut
        }

        // Anchored position
        Vector3 enabledPosition;
        // Global position
        Vector3 disabledPosition;
        RectTransform rt;

        public bool EnabledOnStart = false;
        public Direction TransitionDirection = Direction.Down;
        public UnityEvent CloseAction;
        public float Duration = 0.5f;

        [HideInInspector]
        public State state = State.In;

        float animationStartTime = 0f;

        public void Enable()
        {
            state = State.MovingIn;
            StartCoroutine(Move(enabledPosition, true, State.In));
        }

        public void Disable()
        {
            state = State.MovingOut;
            StartCoroutine(Move(disabledPosition, false, State.Out));
        }

        private void Start()
        {
            SafeAreaScaler.onResolutionChange.AddListener(StartDelayedPositionUpdate);

            rt = GetComponent<RectTransform>();

            enabledPosition = rt.anchoredPosition;
            Debug.Log($"Enabled position: {rt.anchoredPosition}");

            StartDelayedPositionUpdate();
        }

        void CalculateDisabledPosition()
        {
            // Extra distance over the screen edge to make sure the element is completely hidden
            float margin = 256f;

            Vector3 prevPos = rt.position;
            rt.anchoredPosition = enabledPosition;
            if (rt.position != prevPos)
            {
                Debug.Log($"State: {state}, moved from {prevPos} to {rt.position}. Anchored Pos: {rt.anchoredPosition}");
            }

            switch (TransitionDirection)
            {
                case Direction.Up:
                    disabledPosition = rt.position + Vector3.up * (Screen.height + margin);
                    break;
                case Direction.Down:
                    disabledPosition = rt.position + Vector3.down * (Screen.height + margin);
                    break;
                case Direction.Right:
                    disabledPosition = rt.position + Vector3.right * (Screen.width + margin);
                    break;
                case Direction.Left:
                    disabledPosition = rt.position + Vector3.left * (Screen.width + margin);
                    break;
                default:
                    break;
            }

            if (state == State.Out)
            {
                rt.position = disabledPosition;
            }
        }

        /// <summary>
        /// Delays the calculations by one frame to wait for SafeAreaScaler's scalings to apply
        /// </summary>
        void StartDelayedPositionUpdate()
        {
            StartCoroutine(DelayedPositionUpdate());
        }

        IEnumerator DelayedPositionUpdate()
        {
            yield return null;
            CalculateDisabledPosition();
        }

        IEnumerator Move(Vector3 targetPos, bool local, State nextState)
        {
            animationStartTime = Time.time;
            Vector3 startPos = local ? (Vector3)rt.anchoredPosition : rt.position;
            float t = Time.time - animationStartTime;

            while (t < Duration)
            {
                t = Time.time - animationStartTime;
                if (local) 
                    rt.anchoredPosition = Vector3.Lerp(startPos, targetPos, t / Duration);
                else
                    rt.position = Vector3.Lerp(startPos, targetPos, t / Duration);
                yield return null;
            }
            if (local)
                rt.anchoredPosition = targetPos;
            else
                rt.position = targetPos;
            state = nextState;
        }

        private void Update()
        {
            if (state == State.In && Input.GetKeyDown(KeyCode.Escape))
            {
                if (CloseAction != null)
                    CloseAction.Invoke();
            }
        }
    }
}
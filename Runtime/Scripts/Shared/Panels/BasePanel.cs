using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.Core
{
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private float _transitionDuration = 0.2f;
        public GameState State;

        public virtual void ShowPanel()
        {
            if (_canvasGroup == null)
            {
                Debug.LogError("CanvasGroup is not assigned!");
                return;
            }
            
            _canvasGroup.DOFade(1f, _transitionDuration).OnComplete(() =>
            {
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;

                OnShowCompleted();
            });

        }
        
        public virtual void HidePanel()
        {
            if (_canvasGroup == null)
            {
                Debug.LogError("CanvasGroup is not assigned!");
                return;
            }

            _canvasGroup.DOFade(0f, _transitionDuration).OnComplete(() =>
            {
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;

                OnHideCompleted();
            });

        }

        // Override this method for custom behavior when the screen is shown
        protected virtual void OnShowCompleted() { }

        // Override this method for custom behavior when the screen is hidden
        protected virtual void OnHideCompleted() { }

        /*
        private IEnumerator FadeIn()
        {
            Debug.Log("FadeIn coroutine started");
            float elapsedTime = 0f;
            while (elapsedTime < _transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / _transitionDuration);
                Debug.Log($"FadeIn: alpha = {_canvasGroup.alpha}");
                yield return null;
            }
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _routineShow = null;
            Debug.Log("FadeIn complete, calling OnShow()");

            OnShowCompleted();
        }

        private IEnumerator FadeOut()
        {            
            float elapsedTime = 0f;
            while (elapsedTime < _transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / _transitionDuration);
                yield return null;
            }
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            _routineHide = null;

            OnHideCompleted();
        }
        */
    }
}
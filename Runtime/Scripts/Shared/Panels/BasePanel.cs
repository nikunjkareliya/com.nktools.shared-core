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
        
        private Coroutine _fadeInCoroutine;
        private Coroutine _fadeOutCoroutine;

        public virtual void ShowPanel()
        {
            if (_canvasGroup == null)
            {
                Debug.LogError("CanvasGroup is not assigned!");
                return;
            }
            
            // Stop any existing fade out coroutine
            if (_fadeOutCoroutine != null)
            {
                StopCoroutine(_fadeOutCoroutine);
                _fadeOutCoroutine = null;
            }
            
            // Start fade in coroutine
            _fadeInCoroutine = StartCoroutine(FadeInCoroutine());
        }
        
        public virtual void HidePanel()
        {
            if (_canvasGroup == null)
            {
                Debug.LogError("CanvasGroup is not assigned!");
                return;
            }

            // Stop any existing fade in coroutine
            if (_fadeInCoroutine != null)
            {
                StopCoroutine(_fadeInCoroutine);
                _fadeInCoroutine = null;
            }
            
            // Start fade out coroutine
            _fadeOutCoroutine = StartCoroutine(FadeOutCoroutine());
        }

        // Override this method for custom behavior when the screen is shown
        protected virtual void OnShowCompleted() { }

        // Override this method for custom behavior when the screen is hidden
        protected virtual void OnHideCompleted() { }

        private IEnumerator FadeInCoroutine()
        {
            float elapsedTime = 0f;
            float startAlpha = _canvasGroup.alpha;
            
            while (elapsedTime < _transitionDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / _transitionDuration;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, progress);
                yield return null;
            }
            
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _fadeInCoroutine = null;

            OnShowCompleted();
        }

        private IEnumerator FadeOutCoroutine()
        {            
            float elapsedTime = 0f;
            float startAlpha = _canvasGroup.alpha;
            
            while (elapsedTime < _transitionDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / _transitionDuration;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);
                yield return null;
            }
            
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            _fadeOutCoroutine = null;

            OnHideCompleted();
        }
    }
}
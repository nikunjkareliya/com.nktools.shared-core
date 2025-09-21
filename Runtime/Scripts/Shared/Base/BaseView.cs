using System.Collections;
using UnityEngine;

namespace Shared.Core
{
    public abstract class BaseView : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup _canvasGroup;        
        private float _transitionSpeed = 0.25f;
        
        private Coroutine _showCoroutine;
        private Coroutine _hideCoroutine;

        public virtual void Show()
        {
            if (_canvasGroup == null)
            {
                Debug.LogError("CanvasGroup is not assigned!");
                return;
            }
            
            // Stop any existing hide coroutine
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }
            
            // Start show coroutine
            _showCoroutine = StartCoroutine(ShowCoroutine());
        }

        public virtual void Hide(bool isInstant = false)
        {
            if (_canvasGroup == null)
            {
                Debug.LogError("CanvasGroup is not assigned!");
                return;
            }
            
            // Stop any existing show coroutine
            if (_showCoroutine != null)
            {
                StopCoroutine(_showCoroutine);
                _showCoroutine = null;
            }
            
            // Start hide coroutine
            _hideCoroutine = StartCoroutine(HideCoroutine(isInstant));
        }
        
        private IEnumerator ShowCoroutine()
        {
            float elapsedTime = 0f;
            float startAlpha = _canvasGroup.alpha;
            
            while (elapsedTime < _transitionSpeed)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / _transitionSpeed;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, progress);
                yield return null;
            }
            
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _showCoroutine = null;
        }
        
        private IEnumerator HideCoroutine(bool isInstant)
        {
            if (isInstant)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
                _hideCoroutine = null;
                yield break;
            }
            
            float elapsedTime = 0f;
            float startAlpha = _canvasGroup.alpha;
            
            while (elapsedTime < _transitionSpeed)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / _transitionSpeed;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);
                yield return null;
            }
            
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _hideCoroutine = null;
        }
    }
}

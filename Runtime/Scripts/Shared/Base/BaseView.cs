using DG.Tweening;
using UnityEngine;

namespace Shared.Core
{
    public abstract class BaseView : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup _canvasGroup;        
        private float _transitionSpeed = 0.25f;

        public virtual void Show()
        {
            _canvasGroup.DOFade(1f, _transitionSpeed).OnComplete(() =>
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }).SetUpdate(true);
        }

        public virtual void Hide(bool isInstant = false)
        {
            _canvasGroup.DOFade(0f, isInstant ? 0f : _transitionSpeed).OnComplete(() =>
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }).SetUpdate(true);
        }
    }
}

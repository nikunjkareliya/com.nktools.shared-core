using UnityEngine;

namespace Shared.Core
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
        private RectTransform _rectTransform;

        private RectTransform GetRectTransform()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }


        private void OnEnable()
        {
            Refresh();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Refresh();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                Refresh();
            }
        }


        private void OnRectTransformDimensionsChange()
        {
            Refresh();
        }

        private void Refresh()
        {
            Rect safeArea = Screen.safeArea; //SafeAreaUtil.GetSafeArea();
            if (safeArea != _lastSafeArea)
            {
                ApplySafeArea(safeArea);
            }
        }

        private void ApplySafeArea(Rect r)
        {
            _lastSafeArea = r;

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            var rt = GetRectTransform();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
        }
    }
}
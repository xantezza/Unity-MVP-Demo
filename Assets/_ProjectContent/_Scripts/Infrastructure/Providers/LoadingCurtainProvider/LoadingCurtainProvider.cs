using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.Providers.LoadingCurtainProvider
{
    public class LoadingCurtainProvider : MonoBehaviour, ILoadingCurtainProvider
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Slider _slider;
        private TweenerCore<float, float, FloatOptions> _tweenerCore;

        private void Awake()
        {
            ForceHide();
        }

        public void Show(float tweenDuration = 0.3f)
        {
            _canvas.enabled = true;
            _tweenerCore = _canvasGroup.DOFade(1f, tweenDuration);
        }

        public void ForceShow()
        {
            _tweenerCore?.Kill();
            _canvasGroup.alpha = 1f;
        }

        public void SetProgress01(float value)
        {
            _slider.value = value;
        }

        public void Hide(float tweenDuration = 0.3f)
        {
            _canvasGroup.DOFade(0f, tweenDuration).OnComplete(ForceHide);
        }

        public void ForceHide()
        {
            _tweenerCore?.Kill();
            _canvasGroup.alpha = 0f;
            _canvas.enabled = false;
        }
    }
}
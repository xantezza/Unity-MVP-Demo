namespace Infrastructure.Providers.LoadingCurtainProvider
{
    public interface ILoadingCurtainProvider
    {
        void Show(float tweenDuration = 0.3f);
        void ForceShow();
        void SetProgress01(float value);
        void Hide(float tweenDuration = 0.3f);
        void ForceHide();
    }
}
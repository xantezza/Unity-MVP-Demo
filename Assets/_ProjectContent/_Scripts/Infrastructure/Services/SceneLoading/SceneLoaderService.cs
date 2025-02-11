using System;
using System.Collections;
using Configs.RemoteConfig;
using Cysharp.Threading.Tasks;
using Infrastructure.Providers.LoadingCurtainProvider;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.Logging;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Infrastructure.Services.SceneLoading
{
    [UsedImplicitly]
    public class SceneLoaderService : ISceneLoaderService
    {
        private readonly ICoroutineRunnerService _coroutineRunner;
        private readonly IConditionalLoggingService _conditionalLoggingService;
        private readonly ILoadingCurtainProvider _loadingCurtainProvider;
        private string _cachedSceneGUID;

        public SceneLoaderService(ICoroutineRunnerService coroutineRunner, IConditionalLoggingService conditionalLoggingService, ILoadingCurtainProvider loadingCurtainProvider)
        {
            _loadingCurtainProvider = loadingCurtainProvider;
            _conditionalLoggingService = conditionalLoggingService;
            _coroutineRunner = coroutineRunner;
        }

        public UniTask LoadScene(AssetReference nextSceneName, Action onLoaded = null, bool allowReloadSameScene = false)
        {
            _coroutineRunner.StartCoroutine(LoadSceneByAddressablesCoroutine(nextSceneName, onLoaded, allowReloadSameScene));
            return default;
        }

        private IEnumerator LoadSceneByAddressablesCoroutine(AssetReference nextScene, Action onLoaded, bool allowReloadSameScene)
        {
            if (!allowReloadSameScene && _cachedSceneGUID == nextScene.AssetGUID)
            {
                _conditionalLoggingService.Log("Scene tried to be loaded from itself, loading ignored", LogTag.SceneLoader);
                onLoaded?.Invoke();
                yield break;
            }

            var timePassed = 0f;

            _cachedSceneGUID = nextScene.AssetGUID;

            _loadingCurtainProvider.Show();

            var waitNextScene = Addressables.LoadSceneAsync(nextScene, LoadSceneMode.Single, false);

            while (timePassed < RemoteConfig.Infrastructure.FakeMinimalLoadTime)
            {
                timePassed += Time.unscaledDeltaTime;
                _loadingCurtainProvider.SetProgress01(waitNextScene.PercentComplete);
                yield return null;
            }

            while (!waitNextScene.IsDone)
            {
                yield return null;
            }

            waitNextScene.Result.ActivateAsync();
            _conditionalLoggingService.Log($"Loaded scene: {waitNextScene.Result.Scene.name} \n{nextScene.AssetGUID}", LogTag.SceneLoader);
            onLoaded?.Invoke();
            
            _loadingCurtainProvider.Hide();
        }
    }
}
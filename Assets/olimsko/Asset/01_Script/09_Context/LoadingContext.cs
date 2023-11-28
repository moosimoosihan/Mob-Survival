using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using olimsko;
using System;
using Cysharp.Threading.Tasks;

public class LoadingContext : ContextModel
{
    public Action<float> OnSceneLoadProgressChanged;

    public Action OnSceneLoadStart;
    public Action OnSceneLoadComplete;

    public void LoadSceneAsyncTrigger(string sceneName, Func<UniTask> doWhenSceneAfterLoadAndBeforeDone = null, float delaySuccess = 1)
    {
        LoadSceneAsync(sceneName, doWhenSceneAfterLoadAndBeforeDone, delaySuccess).Forget();
    }

    private async UniTask LoadSceneAsync(string sceneName, Func<UniTask> doWhenSceneAfterLoadAndBeforeDone, float delaySuccess = 1)
    {
        OnSceneLoadStart?.Invoke();
        OnSceneLoadProgressChanged?.Invoke(0f);

        //게임 데이터 초기화
        OSManager.GetService<ContextManager>().GetContext<PlayerContext>().ResetContext();

        await OSManager.GetService<StateManager>().SaveGlobalAsync();

        await UniTask.Delay((int)(delaySuccess * 1000), true);

        var loadScene = SceneManager.LoadSceneAsync("99_Loading", LoadSceneMode.Single);

        await loadScene;

        var loadSceneTask = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        loadSceneTask.allowSceneActivation = false;

        while (loadSceneTask.progress < 0.9f)
        {
            OnSceneLoadProgressChanged?.Invoke(loadSceneTask.progress);

            await UniTask.Yield();
        }

        OnSceneLoadProgressChanged?.Invoke(1f);

        loadSceneTask.allowSceneActivation = true;

        await UniTask.Delay((int)(delaySuccess * 1000), true);

        if (doWhenSceneAfterLoadAndBeforeDone != null)
            await doWhenSceneAfterLoadAndBeforeDone.Invoke();

        await UniTask.WaitUntil(() => loadSceneTask.isDone);

        OnSceneLoadComplete?.Invoke();
    }

}

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneManager : BaseManager
{
    private static SceneManager _instance;

    public static SceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 있는 SceneManager 찾기
                _instance = FindObjectOfType<SceneManager>();

                // 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject go = new GameObject("SceneManager");
                    _instance = go.AddComponent<SceneManager>();
                }
            }

            return _instance;
        }
    }

    public Scene loadScene { get; private set; } 
    private bool _isLoadingScene = false;
    
    public override void Prepare()
    {
    }

    public override void Run()
    {
        
    }
    
    public async UniTask LoadSceneAsync(string eSceneName, Action<bool> callBack)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning($"Scene '{eSceneName}' is already loading. Ignoring duplicate request.");
            callBack?.Invoke(false);
            return;
        }

        _isLoadingScene = true;

        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(eSceneName);
        while (!asyncLoad.isDone)
        {
            await UniTask.Yield(); // 프레임마다 대기
        }
        
        loadScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        _isLoadingScene = false;
        callBack?.Invoke(true);
    }
}

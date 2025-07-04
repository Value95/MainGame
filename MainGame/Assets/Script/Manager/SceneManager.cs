using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class SceneManager : BaseManager
{
    private static SceneManager instance;

    public static SceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬에 있는 GameManager 찾기
                instance = FindObjectOfType<SceneManager>();

                // 없으면 새로 생성
                if (instance == null)
                {
                    GameObject go = new GameObject("SceenManager");
                    instance = go.AddComponent<SceneManager>();
                }

                // 씬 전환 시 파괴되지 않도록 설정
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

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

        _isLoadingScene = false;
        callBack?.Invoke(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : BaseManager
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬에 있는 GameManager 찾기
                instance = FindObjectOfType<UIManager>();

                // 없으면 새로 생성
                if (instance == null)
                {
                    GameObject go = new GameObject("UIManager");
                    instance = go.AddComponent<UIManager>();
                }

                // 씬 전환 시 파괴되지 않도록 설정
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    [SerializeField] 
    private GameObject windowParent;
    [SerializeField] 
    private GameObject popupParent;
    
    private Queue<BaseUI> _windowHistory;
    private Queue<BaseUI> _popupHistory;
    
    public override void Prepare()
    {
    }
    
    public override void Run()
    {
        _windowHistory = new Queue<BaseUI>();
        _popupHistory = new Queue<BaseUI>();
    }
    
    public BaseUI ShowWindow<T>(object[] eParam = null) where T : BaseUI, new()
    {
        T window = new T();
        GameObject windowPrefab = Resources.Load<GameObject>(window.Path());
        GameObject.Instantiate(windowPrefab, windowParent.transform);

        _BaseUiOpen<T>(windowPrefab, eParam);
        
        _windowHistory.Enqueue(window);
        return window;
    }

    public BaseUI ShowPopup<T>(object[] eParam = null) where T : BaseUI, new()
    {
        T popup = new T();
        GameObject windowPrefab = Resources.Load<GameObject>(popup.Path());
        GameObject.Instantiate(windowPrefab, popupParent.transform);

        _BaseUiOpen<T>(windowPrefab, eParam);
        
        _popupHistory.Enqueue(popup);
        return popup;
    }

    private void _BaseUiOpen<T>(GameObject eUI, object[] eParam) where T : BaseUI, new()
    {
        if (eUI == null)
        {
            Debug.Log("eUI is Null");
            return;
        }
        T ui = eUI.GetComponent<T>();
        
        if (ui == null)
        {
            Debug.Log("window is Null");
            return;
        }

        ui.AnimTrigger("In", () =>
        {
            ui.Open(eParam);
        }).Forget();
    }

    public void CloseWindow()
    {
        BaseUI window = _windowHistory.Dequeue();

        if (window == null)
        {
            Debug.Log("window is Null");
            return;
        }

        window.Close();    
        window.AnimTrigger("Out", () =>
        {
            window.CloseAnim();
        });
    }

    public void ClosePopup()
    {
        BaseUI popup = _popupHistory.Dequeue();

        if (popup == null)
        {
            Debug.Log("window is Null");
            return;
        }

        popup.Close();    
        popup.AnimTrigger("Out", () =>
        {
            popup.CloseAnim();
        });
    }
}

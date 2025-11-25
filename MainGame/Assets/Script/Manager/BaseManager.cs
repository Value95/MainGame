using UnityEngine;

public abstract class BaseManager<T> : MonoBehaviour where T: Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 있는 GameManager 찾기
                _instance = FindObjectOfType<T>();

                // 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
    
    // Awake 초기화 호출
    public abstract void Prepare();

    // Start 초기화 호출
    public abstract void Run();
}

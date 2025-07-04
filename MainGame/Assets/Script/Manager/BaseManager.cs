using UnityEngine;

public abstract class BaseManager : MonoBehaviour
{
    // Awake 초기화 호출
    public abstract void Prepare();

    // Start 초기화 호출
    public abstract void Run();
}

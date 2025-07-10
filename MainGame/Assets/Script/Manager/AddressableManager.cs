using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddressableManager : BaseManager
{
    private static AddressableManager _instance;

    public static AddressableManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 있는 AddressableManager 찾기
                _instance = FindObjectOfType<AddressableManager>();

                // 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject go = new GameObject("AddressableManager");
                    _instance = go.AddComponent<AddressableManager>();
                }
            }

            return _instance;
        }
    }
    
    public override void Prepare()
    {
        throw new System.NotImplementedException();
    }

    public override void Run()
    {
        throw new System.NotImplementedException();
    }
}

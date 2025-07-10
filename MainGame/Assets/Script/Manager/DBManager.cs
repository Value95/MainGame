using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBManager : BaseManager
{
    private static DBManager _instance;

    public static DBManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 있는 GameManager 찾기
                _instance = FindObjectOfType<DBManager>();

                // 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject go = new GameObject("DBManager");
                    _instance = go.AddComponent<DBManager>();
                }
            }

            return _instance;
        }
    }
    
    public override void Prepare()
    {
        
    }

    public override void Run()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TableManager : BaseManager
{
    private static TableManager _instance;

    public static TableManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 있는 GameManager 찾기
                _instance = FindObjectOfType<TableManager>();

                // 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject go = new GameObject("TableManager");
                    _instance = go.AddComponent<TableManager>();
                }

                // 씬 전환 시 파괴되지 않도록 설정
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    private Dictionary<Type, object> _tableDic;
    
    public override void Prepare()
    {
        _tableDic = new Dictionary<Type,object>();
    }

    public override void Run()
    {
        _TableLoad();
    }

    public void SetTableDic(Type eType, object eParam)
    {
        if (!_tableDic.TryAdd(eType, eParam))
        {
            DebugEx.Log($"{eType} 중복");
        }
    }

    private void _TableLoad()
    {
        InitializationTable initializationTable = new InitializationTable();
        initializationTable.TableLoad();
    }

    public T GetTable<T>() where T : class
    {
        if (_tableDic.TryGetValue(typeof(T), out var tableData))
        {
            return tableData as T;
        }

        return null;
    }
}

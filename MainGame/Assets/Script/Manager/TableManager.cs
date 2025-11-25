using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TableManager : BaseManager<TableManager>
{
    private Dictionary<Type, ITableData> _tableDic;
    
    public override void Prepare()
    {
        _tableDic = new Dictionary<Type,ITableData>();
    }

    public override void Run()
    {
        _TableLoad();
    }

    public void SetTableDic(Type eType, ITableData eParam)
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

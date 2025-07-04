using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTable<T> : ITableData where T : BaseData, new()
{
    public virtual string FileName { get; }
    protected Dictionary<int, T> DataDic = new Dictionary<int, T>();
    
    public abstract void LoadTableData(string json);

    public void SetData(int id, T data)
    {
        if (!DataDic.TryAdd(id, data))
        {
            DebugEx.Log($"SetData 실패: 이미 존재하는 ID ({id})");
        }
    }

    public T GetData(int id)
    {
        if (DataDic.TryGetValue(id, out var value))
        {
            return value;
        }

        return null;
    }
}
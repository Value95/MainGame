using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITableData
{
    string FileName { get; }
    void LoadTableData(string json);
}

public class InitializationTable
{
    public void TableLoad()
    {
        TableManager.Instance.SetTableDic(typeof(CharacterTable), TableLoad(new CharacterTable()));
    }

    private ITableData TableLoad(ITableData table)
    {
        string fileName = table.FileName;

        TextAsset jsonAsset = Resources.Load<TextAsset>($"Table/{fileName}");
        if (jsonAsset == null)
        {
            Debug.LogError($"파일을 찾을 수 없음: {fileName}");
            return null;
        }
        
        table.LoadTableData(jsonAsset.text);
        return table;
    }
}

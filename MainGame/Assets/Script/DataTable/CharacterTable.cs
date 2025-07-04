using Defective.JSON;

public class CharacterTable : BaseTable<CharacterData>
{
    public override string FileName { get { return "CharacterTableData"; } }
    
    public override void LoadTableData(string json)
    {
        JSONObject root = new JSONObject(json);

        foreach (JSONObject node in root.list)
        {
            JSONObject jsonObj = node as JSONObject;
            if (jsonObj == null)
            {
                DebugEx.LogWarning("JSONObject로 변환 실패");
                continue;
            }

            CharacterData data = new CharacterData();
            data.Deserialize(jsonObj);

            SetData(data.ID, data);
        }

        DebugEx.Log($"CharacterTable 로딩 완료: {DataDic.Count}개 항목");
    }
}

public class CharacterData : BaseData
{
    public int index;
    public int ID;
    public string Name;

    public override void Deserialize(JSONObject json)
    {
        index = (int)json["index"].intValue; 
        ID = (int)json["ID"].intValue;
        Name = json["Name"].stringValue;
    }
}
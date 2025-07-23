using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseManager
{
   private static GameManager _instance;

   public static GameManager Instance
   {
      get
      {
         if (_instance == null)
         {
            // 씬에 있는 GameManager 찾기
            _instance = FindObjectOfType<GameManager>();

            // 없으면 새로 생성
            if (_instance == null)
            {
               GameObject go = new GameObject("GameManager");
               _instance = go.AddComponent<GameManager>();
            }
         }

         return _instance;
      }
   }

   private void Awake()
   {
      GameManager.Instance.Prepare();
      SceneManager.Instance.Prepare();
      UIManager.Instance.Prepare();
      TableManager.Instance.Prepare();
      NetworkManager.Instance.Prepare();
   }

   private void Start()
   {
      GameManager.Instance.Run();
      SceneManager.Instance.Run();
      UIManager.Instance.Run();
      TableManager.Instance.Run();
      NetworkManager.Instance.Run();
      
      var data = TableManager.Instance.GetTable<CharacterTable>().GetData(50);
      DebugEx.Log(data.ID);
   }

   public override void Prepare()
   {
   }
   
   public override void Run()
   {      
   }


   private void LoadAddresable()
   {
      
   }
}

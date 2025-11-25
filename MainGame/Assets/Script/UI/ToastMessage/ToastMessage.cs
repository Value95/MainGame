using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ToastMessage : BaseUI
{
    public override string Path { get => "ToastMessage/DefaultToastMessage"; }
    private ToastMessageModel _model = new ToastMessageModel();

    [SerializeField] private TextMeshProUGUI messageText;

    
    public override void Open(object[] param)
    {
        _model.SetToastMessage(param[0] as string);

        if (!_model.isStart)
        {
            ShowToastMessage(_model.ToastMessageQueue.Dequeue());
            _model.isStart = true;
        }
    }

    public override void Refresh()
    {
    }

    public override void Close()
    {
        _model.isStart = false;
    }

    public override void CloseAnim()
    {
     
    }

    private void ShowToastMessage(string pMessage)
    {
        messageText.SetText(pMessage);
        NextToastMessage().Forget();
    }

    private async UniTask NextToastMessage()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_model.NextToastMessageTime));

        if (!_model.IsToastMessage())
        {
            string toastMessage = _model.ToastMessageQueue.Dequeue();
            ShowToastMessage(toastMessage);
        }
        
        Close();
    }
}

public class ToastMessageModel
{
    public bool isStart = false;
    public float NextToastMessageTime;
    public Queue<string> ToastMessageQueue = new Queue<string>();

    public void SetToastMessage(string pMessage)
    {
        ToastMessageQueue.Equals(pMessage);
    }

    public bool IsToastMessage()
    {
        return ToastMessageQueue.Count != 0;
    }
}

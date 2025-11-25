using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DefaultPopup : BaseUI
{
    public override string Path { get => "Popup/DefaultPopup"; }

    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI documentText;
    
    public override void Open(object[] param)
    {
        
    }

    public override void Refresh()
    {
        
    }

    public override void Close()
    {
        
    }

    public override void CloseAnim()
    {
        
    }

    public void ShowYesNo(string pTitle, string pDocument,
        Action pYesCallBack = null, Action pNoCallBack = null)
    {
        yesButton.gameObject.SafeSetActive(true);
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { pYesCallBack?.Invoke(); });
        
        noButton.gameObject.SafeSetActive(false);
        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => { pNoCallBack?.Invoke(); });
        
        cancelButton.gameObject.SafeSetActive(false);
        
        ShowText(pTitle, pDocument);
    }
    
    public void ShowYes(string pTitle, string pDocument,
        Action pYesCallBack = null)
    {
        yesButton.gameObject.SafeSetActive(true);
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { pYesCallBack?.Invoke(); });
        
        noButton.gameObject.SafeSetActive(false);
        cancelButton.gameObject.SafeSetActive(false);
        
        ShowText(pTitle, pDocument);
    }

    public void ShowCancel(string pTitle, string pDocument,
        Action pCancelCallBack = null)
    {
        yesButton.gameObject.SafeSetActive(false);
        noButton.gameObject.SafeSetActive(false);
        
        cancelButton.gameObject.SafeSetActive(true);
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => { pCancelCallBack?.Invoke(); });
        
        ShowText(pTitle, pDocument);
    }

    private void ShowText(string pTitle, string pDocument)
    {
        titleText.SetText(pTitle);
        documentText.SetText(pDocument);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private UiAnimator uiAnim;
    
    public abstract string Path();
    public abstract void Open(object[] param); //
    public abstract void Refresh();
    public abstract void Close();
    public abstract void CloseAnim();

    
    public async UniTask AnimTrigger(string eTriggerName, Action eCallBack)
    {
        anim.SetTrigger(eTriggerName);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        uiAnim.ChanageState(UIAnimTimeLineWindow.AnimState.Playing, (isBool) =>
        {
            eCallBack?.Invoke();
        });
    }
}

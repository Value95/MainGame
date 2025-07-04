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
        
        // // 애니메이션이 전환될 때까지 대기
        // while (!stateInfo.IsName(eTriggerName))
        // {
        //     await UniTask.Yield();
        //     stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        // }
        //
        // // 애니메이션이 재생되는 동안 대기
        // while (stateInfo.normalizedTime < 1.0f)
        // {
        //     await UniTask.Yield();
        //     stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        // }
        //
        // eCallBack?.Invoke();
    }
}

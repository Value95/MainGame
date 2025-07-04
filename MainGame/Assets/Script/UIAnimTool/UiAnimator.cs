
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class UiAnimator : MonoBehaviour
{
    
    [SerializeField][HideInInspector]
    public float drawViewMin = 0;
    
    [SerializeField][HideInInspector]
    public float drawViewMax = 1.5f;
    
    [SerializeField][Min(0.1f)]
    public float animTime =0.1f; // 애니메이션의 총 시간
    
    [SerializeField]
    public bool autoPlay;

    public float curTime { get; private set; }

    public Clip drawClip { get; private set; } // 현재 보여주고 있는 클립

    public UIAnimTimeLineWindow.AnimState animState { get; set; } = UIAnimTimeLineWindow.AnimState.None;
    
    [SerializeField] [HideInInspector]
    private List<AnimatorClipGroup> _animationClipGroups;
    
    private Action stateFunc;
    private Action<bool> endCallBack;

    private void Start()
    {
        if (autoPlay)
        {
            stateFunc = Playing;
        }
    }

    private void Update()
    {
        StateUpdate();
    }

    public void StateUpdate()
    {
        if (stateFunc != null)
        {
            stateFunc();
        }
    }

    public void ChanageState(UIAnimTimeLineWindow.AnimState pAnimState, Action<bool> callBack = null)
    {
        if(pAnimState == UIAnimTimeLineWindow.AnimState.Pause || animState != UIAnimTimeLineWindow.AnimState.Pause)
            curTime = 0;

        endCallBack = callBack;
        animState = pAnimState;
        
        switch (pAnimState)
        {
            case UIAnimTimeLineWindow.AnimState.None:
                stateFunc = null;
                break;
            case UIAnimTimeLineWindow.AnimState.Playing:
                stateFunc = Playing;
                break;
            case UIAnimTimeLineWindow.AnimState.Pause:
                stateFunc = Pause;
                break;
            case UIAnimTimeLineWindow.AnimState.ReSet:
                stateFunc = ReSet;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pAnimState), pAnimState, null);
        }
    }

    public void Play(float pPlayTime)
    {
        if (_animationClipGroups == null)
            return;
        
        int lClipGroupCount = _animationClipGroups.Count;
        for (int lClipGroupIndex = 0; lClipGroupIndex < lClipGroupCount; ++lClipGroupIndex)
        {
            foreach (var clipGroup in _animationClipGroups[lClipGroupIndex].clipGroups)
            {
                int lClipListCount = clipGroup.clipList.Count;
                for (int lClipListIndex = 0; lClipListIndex < lClipListCount; ++lClipListIndex)
                {                                                                                                                                                                                                                                                                                                     
                    if (pPlayTime > clipGroup.clipList[lClipListIndex].startTime && pPlayTime < clipGroup.clipList[lClipListIndex].endTime)
                    {
                        clipGroup.clipList[lClipListIndex].Play(_animationClipGroups[lClipGroupIndex].obj, pPlayTime);
                    }
                }
            }
        }
    }
    
    public void Playing()
    {
        Play(curTime);
        
        curTime +=  EditorDeltaTime.GetDeltaTime();
        
        if (curTime > animTime)
        {
            stateFunc = null;
            endCallBack?.Invoke(true);
        }
    }

    public void Pause()
    {
        
    }
    
    public void ReSet()
    {
        stateFunc = null;
        curTime = 0;
        
        int lClipGroupCount = _animationClipGroups.Count;
        for (int lClipGroupIndex = 0; lClipGroupIndex < lClipGroupCount; ++lClipGroupIndex)
        {
            foreach (var clipGroup in _animationClipGroups[lClipGroupIndex].clipGroups)
            {
                clipGroup.clipList[0].Reset(_animationClipGroups[lClipGroupIndex].obj);
            }
        }
        ChanageState(UIAnimTimeLineWindow.AnimState.None);
    }
    
    public void AddAnimClipGroup()
    {
        if (_animationClipGroups == null)
            _animationClipGroups = new List<AnimatorClipGroup>();
                     
        var lAnimationClipGroup = new AnimatorClipGroup(_animationClipGroups.Count);
        
        _animationClipGroups.Add(lAnimationClipGroup);
    }

    // 현재 선택된 클릭의 정보를 inspector에 표시
    public void DrawClipInfo()
    {
        if (drawClip != null)
        {
            drawClip.DrawInspector();
        }
    }
    
    public AnimatorClipGroup GetAnimationClipGroup(int pIndex)
    {
        if (_animationClipGroups == null)
            return new AnimatorClipGroup();
        
        return _animationClipGroups[pIndex];
    }

    public int GetAnimationClipGroupCount()
    {
        if (_animationClipGroups == null)
            return 0;
        else
            return _animationClipGroups.Count;
    }

    public void AnimationClipGroupsRemoveAt(int pIndex)
    {
        _animationClipGroups.RemoveAt(pIndex);
    }
    
    public void SetDrawClip(Clip pClip)
    {
        if (drawClip == pClip)
            drawClip = null;
        else
            drawClip = pClip;

        EditorUtility.SetDirty(this);
    }
}


[System.Serializable]
public class ClipGroup
{
    public ClipGroup()
    {
        
    }

    public ClipGroup(Clip.ClipType pCliptype)
    {
        clipType = pCliptype;
        clipList = new List<Clip>();
    }
    

    public Clip.ClipType clipType;
    

    [SerializeField]
    public List<Clip> clipList;
}

[System.Serializable]
public class AnimatorClipGroup
{
    [SerializeField]
    public int index;

    [SerializeField]
    public bool isToogle;

    [SerializeField]
    public GameObject obj = null;

    [SerializeField] 
    public List<ClipGroup> clipGroups;
    
    [SerializeField]
    private string _groupName;
    
    public AnimatorClipGroup() { }

    public AnimatorClipGroup(int pIndex)
    {
        index = pIndex;
        isToogle = true;
        _groupName = "group";
        obj = null;
        clipGroups = new List<ClipGroup>();
    }

    public void IsObjNullCheck()
    {
        if (obj != null)
        {
            if (clipGroups.Count >= 1)
            {
                clipGroups.Clear();
            }
        }
    }
    
    [SerializeField]
    public string groupName
    {
        get
        {
            if (obj == null)
                return _groupName;
            else
                return obj.name;
        }
    }

    public void AddClipGroup(Clip.ClipType pClipType)
    {
        if (clipGroups != null)
        {
            foreach (var clipGroup in clipGroups)
            {
                if (clipGroup.clipType == pClipType)
                {
                    return;
                }
            }
            
            clipGroups.Add(new ClipGroup(pClipType));
        }
    }

    public void AddClip(UiAnimator pUiAnimator, Clip.ClipType pClipType, float pStartTime, float pEndTime)
    {
        Clip lClip = CreateClip(pUiAnimator, pClipType, pStartTime, pEndTime);
        
        if(lClip == null)
            return;
        
        AddClipGroup(pClipType);

        foreach (var clipGroup in clipGroups)
        {
            if (clipGroup.clipType == pClipType)
            {
                clipGroup.clipList.Add(lClip);
            }
        }
    }

    public bool ClipGroupContainsKey(Clip.ClipType pClipType)
    {
        foreach (var clipGroup in clipGroups)
        {
            if (clipGroup.clipType == pClipType)
            {
                return true;
            }
        }

        return false;
    }
    
    private Clip CreateClip(UiAnimator pUiAnimator, Clip.ClipType pClipType, float pStartTime, float pEndTime)
    {
        
        switch (pClipType)
        {
             case Clip.ClipType.Position:
                PositionClip lPositionClip = ScriptableObject.CreateInstance<PositionClip>();
                lPositionClip.Initialize(pUiAnimator, pClipType, pStartTime, pEndTime);
                return lPositionClip;
             
            case Clip.ClipType.Rotation:
                RotationClip lRotationClip = ScriptableObject.CreateInstance<RotationClip>();
                lRotationClip.Initialize(pUiAnimator, pClipType, pStartTime, pEndTime);
                return lRotationClip;
            
            case Clip.ClipType.Scale:
                ScaleClip lScaleClip = ScriptableObject.CreateInstance<ScaleClip>();
                lScaleClip.Initialize(pUiAnimator, pClipType, pStartTime, pEndTime);
                return lScaleClip;
            
            case Clip.ClipType.Active:
                ActiveClip lActiveClip = ScriptableObject.CreateInstance<ActiveClip>();
                lActiveClip.Initialize(pUiAnimator, pClipType, pStartTime, pEndTime);
                return lActiveClip;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(pClipType), pClipType, null);
        }

        return null;
    }
}

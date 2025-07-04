using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{

    public UiAnimator uiAnimator;
    // Start is called before the first frame update
    void Start()
    {
        uiAnimator.ChanageState(UIAnimTimeLineWindow.AnimState.Playing);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

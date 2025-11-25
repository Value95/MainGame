using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : BaseUI
{
    public override string Path { get => "Window/TestWindow"; }

    public override void Open(object[] param)
    {
        DebugEx.Log("Open Window");
    }

    public override void Refresh()
    {
        DebugEx.Log("Refresh");
    }

    public override void Close()
    {
        DebugEx.Log("Close");
    }

    public override void CloseAnim()
    {
        DebugEx.Log("CloseAnim");
    }
}

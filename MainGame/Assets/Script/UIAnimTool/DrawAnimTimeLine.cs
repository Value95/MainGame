using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class DrawAnimTimeLine : EditorWindow
{   
    [SerializeField]
    protected float animTime; // 애니메이션의 총 시간
    [SerializeField]
    protected float currentTime; // 현재 선택한 애니메이션의 시간

    [SerializeField]
    protected float drawViewMin; // 애니메이션 view에서 보여주는 최소값

    [SerializeField]
    private float drawViewMax; // 애니메이션 view에서 보여주는 최대값
    
    protected float DrawViewMax
    {
        get { return Mathf.Min(drawViewMax, animTime); }
        set { drawViewMax = value; }
    }
    
    [SerializeField]
    protected Rect timeLineRect;
    [SerializeField]
    protected Rect contentsRect;
    [SerializeField]
    protected Rect windowRect;

    [SerializeField]
    protected float drawTime; // 현재 view에 보여주고 있는 총시간
    
    [SerializeField]
    private float nextSell; // sell과 sell사이의 값
    
    [SerializeField]
    private int sell = 10; // 0~1 사이의 표시할 칸의 갯수

    void OnGUI()
    {
        windowRect = position;

        var cEvent = Event.current;

        RectSetting(); // TimeLine Rect, ContentRect의 범위를 설정

        _DrawSlider();
        _DrawTimeDot();
        _DrawCurrentTime(); // 현재 클릭한 TimeLine의 시간을 표시

        ContentsUpdate(contentsRect, cEvent);
    }

    protected void Init()
    {
        currentTime = 0;
        
        drawTime = DrawViewMax - drawViewMin;
    }

    private void RectSetting()
    {
        contentsRect = Rect.MinMaxRect(40, 75, 245, windowRect.height + 75);
        timeLineRect = Rect.MinMaxRect(250, 50, windowRect.width + 40, 80);
    }

    private void _DrawSlider()
    {
        Rect lDrawRect = timeLineRect;
        lDrawRect.width -= 50;
        lDrawRect.y -= 40;
        
        BasicDraw.DrawMinMaxSlider(lDrawRect, ref drawViewMin, ref drawViewMax, 0, animTime);
        drawViewMax = Mathf.Max(drawViewMax, drawViewMin + 0.1f);
    }

    float FilterValues(float value)
    {
        while (true)
        {
            if (value < nextSell)
                return value;

            value -= nextSell;
        }
    }

    // 시간을 나타내는 점과 라인을 표시하는 메서드
    private void _DrawTimeDot()
    {
        drawTime = DrawViewMax - drawViewMin;
        nextSell = (drawTime / (drawTime * sell));
       
        float lDrawTime = drawViewMin;

        float lStartTime = FilterValues(drawViewMin) / drawTime;

        float lLoopCount = drawTime + nextSell;
        int lDrawCount = 0;

        // TimeLine Draw
        for (var i = 0.0f; i <= lLoopCount; i += nextSell)
        {
            float lXpos = TimeLineValueToPosition((i / drawTime) - lStartTime);

            Vector3 lStartVector = new Vector3(lXpos, timeLineRect.y, 0);
            Vector3 lEndVector = new Vector3(lXpos, windowRect.height, 0);
            
            if (lXpos >= timeLineRect.x)
            {
                Handles.DrawAAPolyLine(lStartVector, lEndVector);

                BasicDraw.GuiStyleRefresh();
                BasicDraw.labelStyle.alignment = TextAnchor.MiddleCenter;

                if ((int)lDrawTime - lDrawTime == 0)
                {
                    BasicDraw.labelStyle.fontSize = 15;
                }
                else
                {
                    BasicDraw.labelStyle.fontSize = 10;
                }

                float lDrawNumber = (int)((lLoopCount * 10) / 20);
                if (lDrawNumber == 0 || lDrawTime == 0 || 0 == lDrawCount % lDrawNumber)
                {
                    BasicDraw.DrawText(new Rect(lStartVector.x - 10, lStartVector.y - 20, 30, 20), (float)(Math.Floor(lDrawTime * 10f) / 10f), "0.00");
                }

            }

            lDrawTime += 0.1f;
            lDrawCount++;
        }

        // Sub TimeLine Draw
        if (drawTime <= 2.0f) // 2.0 이상으로 보여줄시 지저분해 보여 2.0 이후로는 안그리게 설정
        {
            nextSell *= 0.2f;
            for (var i = 0.0f; i <= lLoopCount; i += nextSell)
            {
                float lXpos = TimeLineValueToPosition((i / drawTime) - lStartTime); // 0 ~ drawTime

                if (lXpos >= timeLineRect.x)
                {
                    Vector3 lStartVector = new Vector3(lXpos, timeLineRect.y + 10, 0);
                    Vector3 lEndVector = new Vector3(lXpos, windowRect.height, 0);

                    BasicDraw.DrawLine(lStartVector, lEndVector);
                }
            }
        }

        // currentTime Draw
        {
            float lXpos = TimeLineValueToPosition( (currentTime) / drawTime);
            if (lXpos >= timeLineRect.x)
            {
                Vector3 lStartVector = new Vector3(lXpos, timeLineRect.y, 0);
                Vector3 lEndVector = new Vector3(lXpos, windowRect.height, 0);

                Handles.color = Color.cyan;
                BasicDraw.DrawLine(lStartVector, lEndVector);

                BasicDraw.labelStyle.normal.textColor = Color.cyan;
                BasicDraw.labelStyle.fontSize = 15;

                BasicDraw.DrawText(new Rect(lStartVector.x - 20, lStartVector.y - 20, 40, 20), currentTime + drawViewMin, "0.00");
            }
        }
    }
   
    private void _DrawCurrentTime()
    {
        float lCurTime = currentTime;

        Rect lDrawRect = timeLineRect;
        // Slider에 선을 그릴때 Handle이 가운대로 보이게 하기 위해서 x -= 5 , width += 15
        lDrawRect.x -= 5;  lDrawRect.width += 15;
        
        lCurTime = BasicDraw.DrawSlider(lDrawRect, lCurTime, 0, drawTime);
        if (lCurTime != currentTime)
            currentTime = lCurTime;
    }


    protected float TimeLineValueToPosition(float pTimeValue) // 0.0f ~ 1.0f
    {
        return timeLineRect.x + ((timeLineRect.width - 50) * pTimeValue);
    }

    protected float PositionToTimeLineValue(float pPositionX)
    {
        // timeLineRect의 x 위치를 기준으로 한 상대적인 위치를 구합니다.
        float relativeX = pPositionX - timeLineRect.x;

        // timeLineRect의 너비에서 50을 뺀 값을 기준으로 한 상대적인 위치의 퍼센트 값을 계산합니다.
        float percentage = relativeX / (timeLineRect.width - 50);
        
        // 계산된 퍼센트 값을 반환합니다.
        return percentage;
    }
    
    // TimeLine 콘텐츠 Update
    protected virtual void ContentsUpdate(Rect pLeftRect, Event pMouseEvent)
    {
    }
}
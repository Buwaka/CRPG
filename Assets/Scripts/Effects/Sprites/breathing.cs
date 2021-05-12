using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breathing : MonoBehaviour
{
    [HideInInspector]
    private Coroutine routine = null;

    public SpriteRenderer target;


    public float LoopTime = 1.0f;




    // Start is called before the first frame update
    void init()
    {
        if (!target)
        {
            SpriteRenderer comp = GetComponent<SpriteRenderer>();
            if (!comp)
                return;
            target = comp;
        }

        routine = StartCoroutine(Breath(target, LoopTime));
    }

    void Start()
    {
        init();
    }

    void OnDisable()
    {
        StopCoroutine(routine);
    }

    void OnEnable()
    {
        init();
    }

    void OnDestroy()
    {
        StopCoroutine(routine);
    }

    // cheaper version of a cos() or sin() wave
    float CheapCurve(float time, float LoopTime = 1.0f, float CurveHeight = 1.0f, bool Negative = false)
    {
        //old
        //per second interval plot (2x%2) * ((-2x)%2) * (1)^floor(x)
        //cheaper version (per 2 seconds): plot (x%2) * ((-x)%2) * (1)^floor(x)
        //c# % doesn't support inverse modulo, so here's a version without (per 1 second)
        //(2x%2) * (2-(2x%2))

        //float a = 2; // "speed", how fast we increment, simple terms the height
        //with a = 2, we'll get a 0 to 1 loop no matter what, then b is just a time coefficient
        //float b = Mathf.Pow((loopTime / a),-1); 
        // a / b = time
        // a/b = (b / a) ^ -1
        //so (time / a) ^ -1


        //new
        //(x/t%1) * (1-x/t%1) * h * 4
        //x(1−x) is symmetric around x=1/2, and that's where the peak is, so the peak height is 1/2 × 1/2 = 1/4.

        float temp = (time / LoopTime % 1) * (1 - time / LoopTime % 1) * CurveHeight * 4;

        if (Negative)
        {
            temp *= Mathf.Pow(-1, Mathf.Floor(time));
        }

        return temp;
    }

    float CosCurve(float time, float LoopTime = 1.0f, float CurveHeight = 1.0f, bool Negative = false)
    {
        float temp = Mathf.Cos(time / (LoopTime / Mathf.PI)) * CurveHeight;

        if (!Negative)
        {
            temp = Mathf.Abs(temp);
        }

        return temp;
    }

    IEnumerator Breath(SpriteRenderer renderer, float time = 1.0f, float height = 1.0f)
    {
        //old
        //per second interval plot (2x%2) * ((-2x)%2) * (1)^floor(x)
        //cheaper version (per 2 seconds): plot (x%2) * ((-x)%2) * (1)^floor(x)
        //c# % doesn't support inverse modulo, so here's a version without (per 1 second)
        //(2x%2) * (2-(2x%2))

        //float a = 2; // "speed", how fast we increment, simple terms the height
        //with a = 2, we'll get a 0 to 1 loop no matter what, then b is just a time coefficient
        //float b = Mathf.Pow((loopTime / a),-1); 
        // a / b = time
        // a/b = (b / a) ^ -1
        //so (time / a) ^ -1


        //new
        //(x/t%1) * (1-x/t%1) * h * 4
        //x(1−x) is symmetric around x=1/2, and that's where the peak is, so the peak height is 1/2 × 1/2 = 1/4.

        Color Base = renderer.color;

        while (true)
        {
            float x = Time.time;
            Base.a = (x / time % 1) * (1 - x / time % 1) * height * 4;
            renderer.color = Base;
            yield return new WaitForFixedUpdate();
        }
    }
}

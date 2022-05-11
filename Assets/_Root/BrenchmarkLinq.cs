using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Pancake.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BrenchmarkLinq : MonoBehaviour
{
    [SerializeField] private int loopCount = 100000;

    private int[] _array = new int[1024];
    private List<int> _list = new List<int>(1024);
    private Func<int, bool> _checkOne = _ => _ == 1;
    private Func<int, bool> _checkZero = _ => _ == 0;
    private Func<int, bool> _checkGreaterZero = _ => _ > 0;

    private Predicate<int> _predicateOne = i => i == 1;
    private Predicate<int> _predicateZero = i => i == 0;
    private Predicate<int> _predicateGreaterZero = i => i > 0;
    private Stopwatch _stopwatch = new Stopwatch();
    private string report;

    public void Start()
    {
        Aggregate();
        Any();
        All();
        Averange();
        Contains();
        Count();
        /*Distinct();*/
        First();
        Last();
        Max();
        Min();
        OrderBy();
        Range();
        Repeat();
        Reverse();
        Select();
        Single();
        Skip();
        Sum();
        Take();
        Where();
        Zip();
    }


    public void Aggregate()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            int result = _array.Aggregate((i1, i2) => i1 + 1);
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            int result = _array.AggregateF((i1, i2) => i1 + 1);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;

        var str = "[Aggregate]:         " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Aggregate]: " + duration + "     F: " + durationF);
    }

    public void Any()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Any(_checkOne);
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.AnyF(_predicateOne);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;

        var str = "[Any]:           " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Any]: " + duration + "     F: " + durationF);
    }

    public void All()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.All(_checkZero); // bad case
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.AllF(_predicateZero);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[All]:           " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[All]: " + duration + "     F: " + durationF);
    }

    public void Averange()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Average();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.AverageF();
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Averange]:          " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Averange]: " + duration + "     F: " + durationF);
    }

    public void Contains()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Contains(1);
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.ContainsF(1);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Contains]:          " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Contains]: " + duration + "     F: " + durationF);
    }

    public void Count()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Count(_checkZero);
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.CountF(_checkZero);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Count]:             " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Count]: " + duration + "     F: " + durationF);
    }

    public void First()
    {
        _array[1023] = 1;
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.First(_checkOne);
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.FirstF(_checkOne);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;

        _array[1023] = 0;
        var str = "[First]:         " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[First]: " + duration + "     F: " + durationF);
    }

    public void Last()
    {
        _array[0] = 1;
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Last(_checkGreaterZero);
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.LastF(_predicateZero);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        _array[0] = 0;
        var str = "[Last]:          " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Last]: " + duration + "     F: " + durationF);
    }

    public void Max()
    {
        _array[1023] = 1;
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Max(_checkGreaterZero);
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.MaxF(_checkGreaterZero);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        _array[1023] = 0;
        var str = "[Max]:           " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Max]: " + duration + "     F: " + durationF);
    }

    public void Min()
    {
        _array[1023] = 1;
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Min(_checkGreaterZero);
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.MinF(_checkGreaterZero);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        _array[1023] = 0;
        var str = "[Min]:           " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Min]: " + duration + "     F: " + durationF);
    }

    public void OrderBy()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.OrderBy(_ => _ + 1).Sum();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.OrderByF(_ => _ + 1).Sum();
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[OrderBy]:           " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[OrderBy]: " + duration + "     F: " + durationF);
    }

    public void Range()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = Enumerable.Range(1, 10).ToArray();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = R.RangeArrayF(1, 10);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Range]:         " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Range]: " + duration + "     F: " + durationF);
    }

    public void Repeat()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = Enumerable.Repeat("I like programming.", 15).ToArray();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = R.RepeatArrayF("I like programming.", 15);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Repeat]:            " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Repeat]: " + duration + "     F: " + durationF);
    }

    public void Reverse()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Reverse().ToArray();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.ReverseF();
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Reverse]:           " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Reverse]: " + duration + "     F: " + durationF);
    }

    public void Select()
    {
        _array[1023] = 1;
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Select(_=>_ * _).ToArray();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.SelectF(_=>_ * _);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Select]:            " + duration + "     F: " + durationF;
        report += str + "\n";
        _array[1023] = 0;
        Debug.Log("[Select]:            " + duration + "     F: " + durationF);
    }
    
    public void Single()
    {
        _array[1023] = 1;
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Single(_checkOne);
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.SingleF(_checkOne);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        _array[1023] = 0;
        var str = "[Single]:            " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Single]:            " + duration + "     F: " + durationF);
    }

    public void Skip()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Skip(1023).ToArray();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Skip(1023);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Skip]:          " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Skip]:          " + duration + "     F: " + durationF);
    }

    public void Sum()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Sum();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.SumF();
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Sum]:           " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Sum]:           " + duration + "     F: " + durationF);
    }

    public void Take()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Take(1023).ToArray();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.TakeF(1023);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Take]:          " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Take]:          " + duration + "     F: " + durationF);
    }

    public void Where()
    {
        _array[1023] = 1;
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Where(_checkOne).ToArray();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.WhereF(_checkOne);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        _array[1023] = 0;
        var str = "[Where]:         " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Where]:         " + duration + "     F: " + durationF);
    }

    public void Zip()
    {
        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.Zip(_array, (i1, i2) => i1 + i2).ToArray();
        }

        long duration = _stopwatch.ElapsedMilliseconds;

        _stopwatch.Reset();
        _stopwatch.Restart();
        for (int i = 0; i < loopCount; i++)
        {
            var result = _array.ZipF(_array, (i1, i2) => i1 + i2);
        }

        long durationF = _stopwatch.ElapsedMilliseconds;
        var str = "[Zip]:           " + duration + "     F: " + durationF;
        report += str + "\n";
        Debug.Log("[Zip]:           " + duration + "     F: " + durationF);
    }
    
    void OnGUI()
    {
        GUI.TextArea(new Rect(0, 0, Screen.width, Screen.height), report);
    }
}
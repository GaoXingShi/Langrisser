using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomManager
{
    private int rep = 0;
    private static object syncRoot = new UnityEngine.Object();
    private static volatile RandomManager instance; 
    public static RandomManager Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new RandomManager();
                    }
                }
            }
            return instance;
        }
    }
    
    /// <summary>
    /// 几率 eg:(1,10) 就是百分之10概率
    /// </summary>
    /// <param name="_value1">概率</param>
    /// <param name="_value2">总数</param>
    /// <returns></returns>
    public bool Probability(int _value1,int _value2)
    {
        int result = Random.Range(1, _value2);
        return result < _value1;
    }
}


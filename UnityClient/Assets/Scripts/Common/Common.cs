using UnityEngine;
using System.Collections;


public sealed class TranslateDataPack : object
{
    public bool isCustomData = true;

    public string data;//for frame data
    public ArrayList customs = new ArrayList(); // for custom data
    public static TranslateDataPack Decode(string msg)
    {
        TranslateDataPack ret = new TranslateDataPack();

        if (msg == null) return null;
        if (msg == "") return null;
        if (msg.Length < 4) return null;// illegal 

        if (msg.Substring(0, 3) == "cmd")
        {
            int last = 4;
            ret.isCustomData = true;
            for (int i = 4; i < msg.Length; i++)
            {
                char ch = msg[i];
                if (ch == ':')
                {
                    ret.customs.Add(msg.Substring(last, i - last));
                    last = i + 1;
                }
            }
            ret.customs.Add(msg.Substring(last));
        }
        else
        {
            ret.isCustomData = false;
            ret.data = msg;
        }
        return ret;
    }
}



public class McmpBalls : IComparer
{
    public int Compare(object x, object y)
    {

      /*  Ball a1 = (x as Ball);
        Ball b1 = (y as Ball);

        if (a1.value > b1.value) return 1;
        if (a1.value == b1.value) return 0;
        */
        return -1;
    }

};




public class StableSort
{
    public static void Sort(ref  ArrayList list, IComparer cmp)
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int ii = 0; ii < list.Count; ii++)
            {
                if (cmp.Compare(list[i], list[ii]) > 0)
                {

                    object tmp = list[ii];
                    list[ii] = list[i];
                    list[i] = tmp;
                }
            }
        }
    }
}




public class Counter
{
    public int counter;
    public void Reset()
    {
        counter = 0;
    }

    public bool Tick()
    {
        counter++;
        return false;
    }

};
 
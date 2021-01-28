using System;
using System.Collections.Generic;
using System.Text;

namespace DicomGrep.Utils
{
    public class Util
    {
        // todo: use LRU
        public static void PushToList(string newItem, IList<string> list, int capacity)
        {
            if (capacity <= 1 )
            {
                throw new ArgumentOutOfRangeException();
            }

            int index = list.IndexOf(newItem);

            if (list.Count == 0)
            {
                list.Add(newItem);
            }
            else
            {
                list.Insert(0, newItem);
            }

            if (index >= 0)
            {
                list.RemoveAt(index + 1);
            }

            while (list.Count > capacity)
            {
                list.RemoveAt(list.Count - 1);
            }

        }
    }
}

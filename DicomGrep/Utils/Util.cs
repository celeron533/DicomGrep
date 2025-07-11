using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DicomGrep.Utils
{
    public class Util
    {
        // todo: use LRU
        public static void PushToList(string newItem, IList<string> list, int capacity)
        {
            if (capacity <= 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            int index = list.IndexOf(newItem);

            if (index < 0)
            {
                list.Insert(0, newItem);
            }
            else if (index == 0)
            {
                // already in the list and it is the 1st item, do nothing
            }
            else
            {
                if (list is ObservableCollection<string> observableCollection)
                {
                    observableCollection.Move(index, 0);
                }
                else
                {
                    list.RemoveAt(index);
                    list.Insert(0, newItem);
                }
            }

            while (list.Count > capacity)
            {
                list.RemoveAt(list.Count - 1);
            }

        }
    }
}

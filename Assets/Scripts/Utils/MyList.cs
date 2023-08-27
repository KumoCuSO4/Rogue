using System.Collections.Generic;

namespace Utils
{
    public class MyList<T>
    {
        private List<T> list = new List<T>();

        public T this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }
    }
}
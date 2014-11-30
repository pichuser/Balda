using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.DataAccess
{
    public class ApplicationStateInMemory : IApplicationState
    {
        static private Dictionary<string, object> _items;
        public static Dictionary<string, object> items
        {
            get
            {

                if (_items == null)
                {
                    _items = new Dictionary<string, object>();
                }
                return _items;
            }
        }
        public ApplicationStateInMemory()
        {
        }
        public object this[string name]
        {
            get
            {
                if (!items.Keys.Contains(name))
                {
                    return null;
                }
                return items[name];
            }
            set
            {
                items[name] = value;
            }
        }

        public void Remove(string name)
        {
            items.Remove(name);
        }


        public void Clear()
        {
            items.Clear();
        }
    }
}

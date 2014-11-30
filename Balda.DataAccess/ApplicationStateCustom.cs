using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Balda.DataAccess
{
    public class ApplicationStateCustom : IApplicationState
    {
        private HttpApplicationState items;
        public ApplicationStateCustom()
        {
            items = System.Web.HttpContext.Current.Application;
        }
        public object this[string name]
        {
            get
            {
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

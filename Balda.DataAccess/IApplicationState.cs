using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.DataAccess
{
    /// <summary>
    /// Для того, чтобы можно было тестировать
    /// </summary>
    public interface IApplicationState
    {
        //
        // Summary:
        //     Gets the value of a single System.Web.HttpApplicationState object by name.
        //
        // Parameters:
        //   name:
        //     The name of the object in the collection.
        //
        // Returns:
        //     The object referenced by name.
        object this[string name] { get; set; }

        //
        // Summary:
        //     Removes the named object from an System.Web.HttpApplicationState collection.
        //
        // Parameters:
        //   name:
        //     The name of the object to be removed from the collection.
        void Remove(string name);

        //
        // Summary:
        //     Removes all objects from an System.Web.HttpApplicationState collection.
        void Clear();
    }
}

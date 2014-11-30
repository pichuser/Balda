using Balda.DataAccess;
using Balda.Services;
using Balda.UserManagement;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.DI
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDataRepository>().To<InSessionDataRepository>();
            Bind<IUserDataStorage>().To<InSessionUserDataStorage>();
            Bind<IUserService>().To<UserService>();
            Bind<UserService>().To<UserService>();
            Bind<IWordService>().To<WordService>();
            Bind<IClock>().To<SystemClock>();
            Bind<IApplicationState>().To<ApplicationStateCustom>();
        }
    }
}

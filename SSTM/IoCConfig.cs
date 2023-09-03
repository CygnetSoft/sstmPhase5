using Autofac;
using Autofac.Integration.Mvc;
using SSTM.Business.Services;
using SSTM.Data.Infrastructure;
using System.Reflection;
using System.Web.Mvc;

namespace SSTM
{
    public class IoCConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            //builder.RegisterType<Logger>().As<ILogger>().InstancePerRequest();
            builder.RegisterType<RepositoryContext>().As<IRepositoryContext>().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(TrainingCenterService).Assembly).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
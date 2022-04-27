using SimpleInjector;

namespace NASRx.IoC
{
    public static class IoCContainer
    {
        public static Container Create()
        {
            var container = new Container();
            container.Options.EnableAutoVerification = false;

            container.RegisterInfra();
            container.RegisterRepository();
            container.RegisterService();
            return container;
        }
    }
}
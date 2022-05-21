using GrpsServer;
using GrpsServer.Infrastructure;

namespace GrpcServer.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chat service is starting. Press 'Esc' to exit.");

            MefManager.Initialize();

            foreach (var service in MefManager.Container.GetExportedValues<IService>())
            {
                service.Start();
            }

            MefManager.Container.GetExportedValue<Logger>().GetLogsAsObservable()
                .Subscribe((x) => Console.WriteLine(x));

            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }
    }
}
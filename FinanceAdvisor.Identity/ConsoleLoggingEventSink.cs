//using Duende.IdentityServer.Events;
//using Duende.IdentityServer.Services;
//using System.Threading.Tasks;

//public class ConsoleLoggingEventSink : IEventSink
//{
//    public Task PersistAsync(Event evt)
//    {
//        if (evt is TokenIssuedSuccessEvent tokenEvent)
//        {
//            Console.WriteLine($"Access token issued: {tokenEvent.Token}");
//        }
//        return Task.CompletedTask;
//    }
//}

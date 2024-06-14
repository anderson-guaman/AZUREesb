using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

class Program
{
    const string connectionString = "Endpoint=sb://consumo-inventario.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c2nMoEfC4wmmq5RRlSwKc0ptoWIlbZNR/+ASbOoHlUU=";
    const string sourceQueueName = "notificaciones_inventario";
    const string targetQueueName = "tickets";

    static async Task Main(string[] args)
    {
        await ProcessMessagesAsync();
    }

    static async Task ProcessMessagesAsync()
    {
        // Crear un cliente de Service Bus
        ServiceBusClient client = new ServiceBusClient(connectionString);

        // Crear un receptor de la cola de notificaciones
        ServiceBusProcessor processor = client.CreateProcessor(sourceQueueName, new ServiceBusProcessorOptions());

        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        // Iniciar el procesamiento de mensajes
        await processor.StartProcessingAsync();

        Console.WriteLine("Presiona cualquier tecla para detener el procesamiento...");
        Console.ReadKey();

        // Detener el procesamiento de mensajes
        await processor.StopProcessingAsync();
        await client.DisposeAsync();
    }

    static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Mensaje recibido: {body}");

        // Generar un ticket (en este ejemplo, simplemente generamos un ID de ticket)
        string ticketId = Guid.NewGuid().ToString();

        // Crear un nuevo cliente y remitente para la cola de tickets
        ServiceBusClient client = new ServiceBusClient(connectionString);
        ServiceBusSender sender = client.CreateSender(targetQueueName);

        // Crear y enviar el mensaje del ticket
        string ticketMessageBody = $"{{ \"ticketId\": \"{ticketId}\", \"sourceMessage\": {body} }}";
        ServiceBusMessage ticketMessage = new ServiceBusMessage(ticketMessageBody);
        await sender.SendMessageAsync(ticketMessage);

        Console.WriteLine($"Ticket generado y enviado: {ticketMessageBody}");

        // Completar el mensaje para que sea eliminado de la cola
        await args.CompleteMessageAsync(args.Message);

        await client.DisposeAsync();
    }

    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Error: {args.Exception.ToString()}");
        return Task.CompletedTask;
    }
}

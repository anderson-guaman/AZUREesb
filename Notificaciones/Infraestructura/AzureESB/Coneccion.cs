using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

class Coneccion
{
    const string connectionString = "Endpoint=sb://consumo-inventario.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c2nMoEfC4wmmq5RRlSwKc0ptoWIlbZNR/+ASbOoHlUU=";
    const string queueName = "notificaciones_inventario";

    static async Task Main(string[] args)
    {
        await SendMessageAsync();
    }

    static async Task SendMessageAsync()
    {
        // Crear un cliente de Service Bus
        ServiceBusClient client = new ServiceBusClient(connectionString);
        ServiceBusSender sender = client.CreateSender(queueName);

        // Crear un mensaje
        string messageBody = "{ \"event\": \"AlarmEvent\", \"details\": \"Detalles del evento\" }";
        ServiceBusMessage message = new ServiceBusMessage(messageBody);

        // Enviar el mensaje
        await sender.SendMessageAsync(message);
        Console.WriteLine($"Mensaje enviado: {messageBody}");

        // Cerrar el cliente
        await client.DisposeAsync();
    }
}


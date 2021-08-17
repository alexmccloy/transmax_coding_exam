using EagleRock.Model;

namespace EagleRock.Amqp
{
    public interface IAmqpInterface
    {
        void PublishEvent(EagleBotPayload payload);
    }
}
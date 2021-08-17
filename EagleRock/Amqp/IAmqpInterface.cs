using EagleRock.Model;

namespace EagleRock.Amqp
{
    /// <summary>
    /// Interface to publish data to AMQP
    /// </summary>
    public interface IAmqpInterface
    {
        /// <summary>
        /// Publish an EagleBot payload to AMQP
        /// </summary>
        /// <param name="payload"></param>
        void PublishEvent(EagleBotPayload payload);
    }
}
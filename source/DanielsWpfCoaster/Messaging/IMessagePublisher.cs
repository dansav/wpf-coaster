using System;

namespace DanielsWpfCoaster.Messaging
{
    public interface IMessagePublisher
    {
        void Publish<TMessage>(TMessage sampleEvent);

        IObservable<TMessage> ObservableFor<TMessage>();
    }
}
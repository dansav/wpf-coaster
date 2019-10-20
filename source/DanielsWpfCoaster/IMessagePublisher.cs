using System;

namespace DanielsWpfCoaster
{
    public interface IMessagePublisher
    {
        void Publish<TMessage>(TMessage sampleEvent);

        IObservable<TMessage> ObservableFor<TMessage>();
    }
}
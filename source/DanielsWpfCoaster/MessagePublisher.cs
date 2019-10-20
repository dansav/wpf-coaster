using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace DanielsWpfCoaster
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly ConcurrentDictionary<Type, object> _subjects;

        public MessagePublisher()
        {
            _subjects = new ConcurrentDictionary<Type, object>();
        }

        public void Publish<TMessage>(TMessage sampleEvent)
        {
            if (_subjects.TryGetValue(typeof(TMessage), out var subject))
            {
                ((ISubject<TMessage>)subject).OnNext(sampleEvent);
            }
        }

        public IObservable<TMessage> ObservableFor<TMessage>()
        {
            var subject = (ISubject<TMessage>)_subjects.GetOrAdd(typeof(TMessage), _ => new Subject<TMessage>());
            return subject.AsObservable();
        }
    }
}

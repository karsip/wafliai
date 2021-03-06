using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.ObjectObserver
{
    public interface ISubject
    {
        void Attach(IObserver observer);

        // Detach an observer from the subject.
        void Detach(IObserver observer);

        // Notify all observers about an event.
        void Notify();

    }
}

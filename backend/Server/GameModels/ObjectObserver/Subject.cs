using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.ObjectObserver
{
    public class Subject: ISubject
    {
        private List<IObserver> _observers = new List<IObserver>();
        private object _lock = new object();
        public void Attach(IObserver observer)
        {
            Console.WriteLine("Subject: Attached an observer.");
            this._observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            this._observers.Remove(observer);
            Console.WriteLine("Subject: Detached an observer.");
        }

        public void Notify()
        {   
            try
            {
                lock (_lock)
                {
                    foreach (var observer in _observers)
                    {
                        observer.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured - " + ex.Message);
            }
        }
    }
}

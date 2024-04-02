

using System.Collections.Concurrent;
using System.Diagnostics;

namespace Semaphores
{
    public enum SemaphoreState 
    {
        Green = 10,
        Yellow = 6,
        Red = 12    
    }

    public class SemaphoreChangeEventArgs(string name, ConsoleColor color) : EventArgs
    {
        public string Name { get; set; } = name;
        public ConsoleColor Color { get; set; } = color;
    }

    public class Watcher
    {
        private IDictionary<string, Semaphore> _semaphores = new ConcurrentDictionary<string, Semaphore>();

        private ConcurrentQueue<SemaphoreChangeEventArgs> _updates = new ConcurrentQueue<SemaphoreChangeEventArgs>();

        private bool _running = false;

        private Task? _process;

        public void Add(Semaphore semaphore)
        {
            _semaphores.Add(semaphore.GetName(), semaphore);
            semaphore.OnSemaphoreChanged += SemaphoreChange;
        }

        public void SemaphoreChange(object sender, SemaphoreChangeEventArgs args)
        {
            _updates.Enqueue(args);
        }

        public void Start()
        {
            if(!_running)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{DateTime.Now} :: Watcher starting!");

                _process = Task.Factory.StartNew(() =>
                {
                    while (this._running)
                    {
                        SemaphoreChangeEventArgs? args;
                        while (_updates.TryDequeue(out args))
                        {
                            Console.ForegroundColor = (ConsoleColor)args.Color;
                            Console.WriteLine($"{DateTime.Now} :: {args.Name} changed to {args.Color}");  
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{DateTime.Now} :: Watcher Stopped!"); 
                });

                _running = true;
            }
        }

        public void Stop()
        {
            _running = false;
            if (_process != null )
            {
                _process.Wait();        
            }
        }
    }

    public delegate void SemaphoreChangedEventHandler(object sender, SemaphoreChangeEventArgs e);

    public class Semaphore(string name)
    {
        private bool _running = false;

        private Task? _process;

        public string GetName()
        {
            return name;
        }

        public event SemaphoreChangedEventHandler? OnSemaphoreChanged;

        public void ChangeState(SemaphoreState newState)
        {   
            if (OnSemaphoreChanged != null) 
            {
                var args = new SemaphoreChangeEventArgs(name, (ConsoleColor)newState);
                OnSemaphoreChanged(this, args);
            }
        }
        public void Start(IDictionary<SemaphoreState, int> config)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{DateTime.Now} :: Semaphore {name} starting!");

            _running = true;
            _process = Task.Factory.StartNew(() =>
            {
                while(this._running)
                {
                    foreach (var item in config)
                    {                                                                        
                        if (!_running)
                        {
                            break;                            
                        }

                        ChangeState(item.Key);
                        Thread.Sleep(item.Value*1000);                                       
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{DateTime.Now} :: Semaphore {name} Stopped!");             
            });
        }

        public void Stop()
        {
            _running = false;

            if (_process != null)
            {
                _process.Wait();
            }
        }
    }
}
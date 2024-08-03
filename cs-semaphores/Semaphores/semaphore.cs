using System.Collections.Concurrent;
using System.Diagnostics;

namespace Semaphores
{
    public enum SemaphoreState 
    {
        Green = 2,
        Yellow = 14,
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
                foreach (var semaphore in _semaphores)
                {
                    semaphore.Value.Start();
                }

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
                            Console.WriteLine($"{DateTime.Now} :: [{args.Name}] ███████████");  
                        }
                    }

                    Console.ResetColor();
                    Console.WriteLine($"{DateTime.Now} :: Watcher Stopped!"); 
                });

                _running = true;
            }
        }

        public void Stop()
        {
            _running = false;
            foreach (var semaphore in _semaphores)
            {
                semaphore.Value.Stop();
            }
            
            if (_process != null )
            {
                _process.Wait();        
            }
        }
    }

    public delegate void SemaphoreChangedEventHandler(object sender, SemaphoreChangeEventArgs e);

    public class Semaphore(string name)
    {
        private IDictionary<SemaphoreState, int> _config = new Dictionary<SemaphoreState, int>();

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
        public void Set(IDictionary<SemaphoreState, int> config)
        {
            _config = config;            
        }

        public void Start()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{DateTime.Now} :: Semaphore {name} starting!");

            _running = true;
            _process = Task.Factory.StartNew(() =>
            {
                while(this._running)
                {
                    foreach (var item in this._config)
                    {                                                                        
                        if (!_running)
                        {
                            break;                            
                        }

                        ChangeState(item.Key);
                        Thread.Sleep(item.Value*1000);                                       
                    }
                }

                Console.ResetColor();
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
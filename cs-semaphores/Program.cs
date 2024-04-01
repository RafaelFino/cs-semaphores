// See https://aka.ms/new-console-template for more information
using Semaphores;

var semA = new Semaphores.Semaphore("Street A");
var semB = new Semaphores.Semaphore("Street B");

var watcher = new Watcher();

watcher.Add(semA);
watcher.Add(semB);

watcher.Start();

semA.Start(new Dictionary<SemaphoreState, int>() {
            {SemaphoreState.Green, 3},
            {SemaphoreState.Yellow, 2},
            {SemaphoreState.Red, 5}
        });

semB.Start(new Dictionary<SemaphoreState, int>() {
            {SemaphoreState.Red, 5},
            {SemaphoreState.Green, 3},
            {SemaphoreState.Yellow, 2}
        });

Console.ReadKey();

semA.Stop();
semB.Stop();
watcher.Stop();
using Semaphores;

var semA = new Semaphores.Semaphore("Street A");
var semB = new Semaphores.Semaphore("Street B");

var watcher = new Watcher();

watcher.Add(semA);
watcher.Add(semB);

watcher.Start();

semA.Start(new Dictionary<SemaphoreState, int>() {
            {SemaphoreState.Green, 2},
            {SemaphoreState.Yellow, 1},
            {SemaphoreState.Red, 3}
        });

semB.Start(new Dictionary<SemaphoreState, int>() {
            {SemaphoreState.Red, 3},
            {SemaphoreState.Green, 2},
            {SemaphoreState.Yellow, 1}
        });

Console.ReadKey();

semA.Stop();
semB.Stop();
watcher.Stop();
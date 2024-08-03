using Semaphores;

var streetA = new Semaphores.Semaphore("Rua da felicidade   ");
var walkA = new Semaphores.Semaphore(  "Boneco feliz        ");
var streetB = new Semaphores.Semaphore("Travessa dos Doencas");
var walkB = new Semaphores.Semaphore(  "Boneco doenca       ");

var watcher = new Watcher();

watcher.Add(streetA);
watcher.Add(streetB);
watcher.Add(walkA);
watcher.Add(walkB);

streetA.Set(new Dictionary<SemaphoreState, int>() {
            {SemaphoreState.Green, 2},
            {SemaphoreState.Yellow, 1},
            {SemaphoreState.Red, 3}
        });

walkA.Set(new Dictionary<SemaphoreState, int>() {
            {SemaphoreState.Red, 3},
            {SemaphoreState.Green, 3}
        });

streetB.Set(new Dictionary<SemaphoreState, int>() {
            {SemaphoreState.Red, 3},
            {SemaphoreState.Green, 2},
            {SemaphoreState.Yellow, 1}
        });

walkB.Set(new Dictionary<SemaphoreState, int>() {
            {SemaphoreState.Green, 3},
            {SemaphoreState.Red, 3}
        });

watcher.Start();

Console.ReadKey();

watcher.Stop();
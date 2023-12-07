using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Reactive;

namespace Demo;
class Program
{

    public class KeyDigitObservable : IObservable<long>
    {
        private List<IObserver<long>> subscribers;

        public IDisposable Subscribe(IObserver<long> obs)
        {
            subscribers.Add(obs);
            return Disposable.Empty;
        }

        private void Start()
        {
            Task.Run(() =>
            {
                var run = true;
                while (run)
                {
                    var line = Console.ReadLine();
                    if (Int32.TryParse(line, out int input))
                    {
                        if (input == 0)
                            Parallel.ForEach<IObserver<long>>(subscribers, sub => sub.OnCompleted());
                        else
                            Parallel.ForEach<IObserver<long>>(subscribers, sub => sub.OnNext(input));
                    }
                    else
                        Parallel.ForEach<IObserver<long>>(subscribers, sub => sub.OnError(new Exception("eto ne cifir'")));
                }});
        }
        public KeyDigitObservable()
        {
            subscribers = new List<IObserver<long>>();
            Start();
        }
    }

    public class DigitObserver : IObserver<long>
    {
        private long _sum = 0;
        private int _count = 0;
        public void OnNext(long value)
        {
            Console.WriteLine($"Received digit {value}");
            _count++;
            _sum += value;
            Console.WriteLine($"Average value is {(double)_sum / _count}");
        }

        public void OnError(Exception ex)
        {
            Console.WriteLine($"Achtung! Exception {ex.Message}");
        }

        public void OnCompleted()
        {
            Console.WriteLine("Sequence completed");
        }
    }
    static void Main()
    {
        IObservable<long> keyValue = new KeyDigitObservable();
        // 
        Console.WriteLine("Observer created");
        keyValue.Subscribe(new DigitObserver());

        while (true)
        {}
    }
}

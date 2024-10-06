using System;
using System.Threading;

namespace DinningPhilosophers
{
    class Program
    {
        static void Main(string[] args)
        {
            Philosopher philosopher1 = new Philosopher(Table.fork5, Table.fork1, "Philosopher1", 4);
            Philosopher philosopher2 = new Philosopher(Table.fork1, Table.fork2, "Philosopher2", 5);
            Philosopher philosopher3 = new Philosopher(Table.fork2, Table.fork3, "Philosopher3", 6);
            Philosopher philosopher4 = new Philosopher(Table.fork3, Table.fork4, "Philosopher4", 4);
            Philosopher philosopher5 = new Philosopher(Table.fork4, Table.fork5, "Philosopher5", 7);

            new Thread(philosopher1.Think).Start();
            new Thread(philosopher2.Think).Start();
            new Thread(philosopher3.Think).Start();
            new Thread(philosopher4.Think).Start();
            new Thread(philosopher5.Think).Start();

            Console.ReadKey();
        }

    }



    enum PhilosopherState { Eating, Thinking }


    class Philosopher
    {
        public string Name { get; set; }

        public PhilosopherState State { get; set; }

        // визначає кількість безперервних "thinking", поки філософ не буде вважатися голодним
        readonly int StarvationThreshold;

        // визначає праву і ліву виделку для філософа
        public readonly Fork RightFork;
        public readonly Fork LeftFork;

        Random rand = new Random();

        int contThinkCount = 0;

        public Philosopher(Fork rightFork, Fork leftFork, string name, int starvThreshold)
        {
            RightFork = rightFork;
            LeftFork = leftFork;
            Name = name;
            State = PhilosopherState.Thinking;
            StarvationThreshold = starvThreshold;
        }

        public void Eat()
        {
            // взяти виделку у праву руку
            if (TakeForkInRightHand())
            {
                // якщо взята виделка у праву руку, негайно спробувати взяти виделку у ліву руку
                if (TakeForkInLeftHand())
                {
                    // якщо є обидві виделки - їсти
                    this.State = PhilosopherState.Eating;
                    Console.WriteLine("(E) {0} is eating with {1} and {2}", Name, RightFork.ForkID, LeftFork.ForkID);
                    Thread.Sleep(rand.Next(5000, 10000));

                    contThinkCount = 0;

                    // покласти виделки назад
                    RightFork.Put();
                    LeftFork.Put();
                }
                // взята виделка тільки у праву руку
                else
                {
                    // чекати невеликий випадковий проміжок часу і спробувати ще раз взяти виделку у ліву руку
                    Thread.Sleep(rand.Next(100, 400));
                    if (TakeForkInLeftHand())
                    {
                        // якщо взята виделка у ліву руку - їсти
                        this.State = PhilosopherState.Eating;
                        Console.WriteLine("(E) {0} is eating with {1} and {2}", Name, RightFork.ForkID, LeftFork.ForkID);
                        Thread.Sleep(rand.Next(5000, 10000));

                        contThinkCount = 0;

                        RightFork.Put();
                        LeftFork.Put();
                    }
                    // якщо не вдалося взяти виделку у ліву руку навіть після очікування, покласти праву виделку назад на стіл
                    else
                    {
                        RightFork.Put();
                    }
                }
            }
            // якщо не вдалося взяти виделку у праву руку
            else
            {
                // взяти виделку у ліву руку
                if (TakeForkInLeftHand())
                {
                    // чекати невеликий випадковий проміжок часу і спробувати взяти виделку у праву руку
                    Thread.Sleep(rand.Next(100, 400));
                    if (TakeForkInRightHand())
                    {
                        // якщо взята виделка у праву руку - їсти
                        this.State = PhilosopherState.Eating;
                        Console.WriteLine("(E) {0} is eating with {1} and {2}", Name, RightFork.ForkID, LeftFork.ForkID);
                        Thread.Sleep(rand.Next(5000, 10000));

                        contThinkCount = 0;

                        RightFork.Put();
                        LeftFork.Put();
                    }
                    else
                    {
                        // якщо не вдалося взяти виделку у праву руку навіть після очікування, покласти ліву виделку назад на стіл
                        LeftFork.Put();
                    }
                }
            }

            Think();
        }

        public void Think()
        {
            this.State = PhilosopherState.Thinking;
            Console.WriteLine("??? {0} is thinking", Name);
            Thread.Sleep(rand.Next(2500, 20000));
            contThinkCount++;

            if (contThinkCount > StarvationThreshold)
            {
                Console.WriteLine("!!!!! {0} is starving", Name);
            }

            Eat();
        }

        private bool TakeForkInLeftHand()
        {
            return LeftFork.Take(Name);
        }

        private bool TakeForkInRightHand()
        {
            return RightFork.Take(Name);
        }

    }




    enum ForkState { Taken, OnTheTable }


    class Fork
    {
        public string ForkID { get; set; }
        public ForkState State { get; set; }
        public string TakenBy { get; set; }

        public bool Take(string takenBy)
        {
            lock (this)
            {
                if (this.State == ForkState.OnTheTable)
                {
                    State = ForkState.Taken;
                    TakenBy = takenBy;
                    Console.WriteLine("||| {0} is taken by {1}", ForkID, TakenBy);
                    return true;
                }

                else
                {
                    State = ForkState.Taken;
                    return false;
                }
            }
        }

        public void Put()
        {
            State = ForkState.OnTheTable;
            Console.WriteLine("||| {0} is placed on the table by {1}", ForkID, TakenBy);
            TakenBy = String.Empty;
        }
    }



    class Table
    {
        internal static Fork fork1 = new Fork() { ForkID = "Fork1", State = ForkState.OnTheTable };
        internal static Fork fork2 = new Fork() { ForkID = "Fork2", State = ForkState.OnTheTable };
        internal static Fork fork3 = new Fork() { ForkID = "Fork3", State = ForkState.OnTheTable };
        internal static Fork fork4 = new Fork() { ForkID = "Fork4", State = ForkState.OnTheTable };
        internal static Fork fork5 = new Fork() { ForkID = "Fork5", State = ForkState.OnTheTable };
    }
}

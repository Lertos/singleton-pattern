using System.Runtime.CompilerServices;
using System.Threading;

namespace singleton_pattern
{
    //A demonstration of the Singleton pattern in C#
    public class Program
    {
        static void Main(string[] args)
        {
            // NAIVE APPROACH
            NaiveSingleton s1 = NaiveSingleton.GetInstance();
            NaiveSingleton s2 = NaiveSingleton.GetInstance();

            Console.WriteLine(s1 == s2); //Output: True

            // THREAD SAFE APPROACH
            Thread process1 = new Thread(() =>
            {
                ThreadSafeSingleton singleton = ThreadSafeSingleton.GetInstance("Thread1");
                Console.WriteLine(singleton.Value);
            });
            Thread process2 = new Thread(() =>
            {
                ThreadSafeSingleton singleton2 = ThreadSafeSingleton.GetInstance("Thread2");
                Console.WriteLine(singleton2.Value);
            });

            process1.Start();
            process2.Start();

            process1.Join();
            process2.Join();

            //OUTPUT:
            //Thread1
            //Thread1
        }
    }

    // NAIVE APPROACH
    public class NaiveSingleton
    {
        //Make sure no one can create a new object using the constructor
        private NaiveSingleton() { }

        //Mark the field as static so there is only one copy of it in memory
        private static NaiveSingleton instance;

        //If the field hasn't been set, initialize it. Always return the initialized instance variable
        public static NaiveSingleton GetInstance()
        {
            if (instance == null)
            {
                instance = new NaiveSingleton();
            }
            return instance;
        }
    }

    // THREAD SAFE APPROACH
    public class ThreadSafeSingleton
    {
        private ThreadSafeSingleton() { }

        private static ThreadSafeSingleton instance;
        //Just for testing purposes
        public string Value { get; set; }

        //We now have a lock object that will be used to synchronize threads during first access to the Singleton
        private static readonly object threadLock = new object();

        public static ThreadSafeSingleton GetInstance(string value)
        {
            if (instance == null)
            {
                //On launch there's no Singleton instance yet, multiple threads can simultaneously pass the
                //previous conditional and reach this point almost at the same time. The first of them will acquire
                //lock and will proceed further, while the rest will wait here
                lock (threadLock)
                {
                    //The first thread to acquire the lock, reaches this conditional, goes inside and creates the
                    //Singleton instance. Once it leaves the lock block, a thread that might have been waiting for
                    //the lock release may then enter this section. The Singleton field will already be initialized,
                    //so the thread won't create a new object
                    if (instance == null)
                    {
                        instance = new ThreadSafeSingleton();
                        instance.Value = value;
                    }
                }
            }
            return instance;
        }
    }
}

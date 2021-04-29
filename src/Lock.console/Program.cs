using System;
using System.Threading;

namespace Lock.console
{
    class Program
    {
        private static readonly object myLock=new object();
        static void Main(string[] args)
        {
            Monitor.Enter(myLock);
            Console.WriteLine("Hello World");
            Monitor.Exit(myLock);
            Console.WriteLine("asdf");
            Console.WriteLine("");
        } 
    }
}

using System;

namespace EnumToString
{
    enum FunWithEnum
    {
        One = 1,
        Two = 3,
        Three = 2,
        Four = 1
    }

    public partial class FakeNum
    {
        public static FakeNumField One = 1;
        public static FakeNumField Two = 0;
        public static FakeNumField Three = 2;
        public static FakeNumField Four = 1;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var fakeNum = new FakeNum();
            
            Console.WriteLine("FAKE ENUM");
            Console.WriteLine(FakeNum.One.ToString());
            Console.WriteLine(FakeNum.Two);
            Console.WriteLine(FakeNum.Three);
            Console.WriteLine(FakeNum.Four);

            Console.WriteLine();
            
            Console.WriteLine("REAL ENUM");
            Console.WriteLine(FunWithEnum.One);
            Console.WriteLine(FunWithEnum.Two);
            Console.WriteLine(FunWithEnum.Three);
            Console.WriteLine(FunWithEnum.Four);
            
            Console.ReadLine();
        }
    }
}

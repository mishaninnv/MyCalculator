using System;

namespace Calculator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Введите выражение:");      

                var expression = Console.ReadLine();
                var result = 0.0;

                if (!string.IsNullOrEmpty(expression))
                {
                    try
                    {
                        result = ExprLib.PolandRevertMath(expression);
                    }
                    catch (ArgumentException exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Ничего не введено");
                }

                Console.WriteLine("********");
                Console.WriteLine("Ответ: " + result);
                Console.WriteLine("Нажмите Enter для продолжения.");
                Console.WriteLine("Нажмите ESC для выхода.");
                Console.WriteLine("********");

            } while (IsContinue());
        }

        private static bool IsContinue()
        {
            while (true)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Enter:
                        return true;
                    case ConsoleKey.Escape:
                        return false;
                }
            }
        }
    }
}

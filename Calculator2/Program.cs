using System;

namespace Calculator2
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Введите выражение:");

                var expression = Console.ReadLine();
                var result = "0.0";

                try
                {
                    var expressionHandler = new ExpressionHandler(expression);
                    var algorithmRpn = new AlgorithmRPN(expressionHandler);
                    result = algorithmRpn.Result;
                }
                catch (ArgumentException exception)
                {
                    ShowResult(exception.Message);
                    continue;
                }

                ShowResult("Ответ: " + Math.Round(double.Parse(result), 2));
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

        private static void ShowResult(string answer)
        {
            Console.WriteLine("********");
            Console.WriteLine(answer);
            Console.WriteLine("Нажмите Enter для продолжения.");
            Console.WriteLine("Нажмите ESC для выхода.");
            Console.WriteLine("********");
        }
    }
}
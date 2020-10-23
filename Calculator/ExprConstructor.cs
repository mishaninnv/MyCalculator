using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Calculator
{
    internal static class ExprConstructor
    {
        private static readonly List<char> OperandValues =
            new List<char> {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ',', '.'};

        private static readonly Dictionary<char, int> OperatorValues =
            new Dictionary<char, int>
            {
                {'(', 0},
                {')', 1},
                {'+', 2},
                {'-', 2},
                {'*', 3},
                {'/', 3}
            };

        private static readonly List<char> AvailableSymbols = new List<char> {'-', '(', '+'};

        /// <summary>
        /// Collects operators and operands from characters of an expression entered by a user (Element).
        /// Writes the received elements to an array.
        /// </summary>
        /// <param name="exprText">Expression in the form of string.</param>
        /// <exception cref="ArgumentException">Throw when expression contains invalid characters.</exception>
        /// <returns>
        /// List consisting of Elements recorded by RPN.
        /// </returns>
        public static IEnumerable<Element> ExprToArr(string exprText)
        {
            ExpressionValidation(exprText);

            var resultRpn = new List<Element>();
            var stackOperations = new Stack<char>();

            var separator = FormatToCulture();
            exprText = separator.Equals('.') ? exprText.Replace(',', '.') : exprText.Replace('.', ',');

            var countParentheses = 0;
            var intermediateValue = new StringBuilder();
            var lastElement = '0';

            foreach (var element in exprText.Where(element => element != ' '))
            {
                if (OperandValues.Contains(element))
                {
                    if (element == separator && !CheckForOperands(element, separator, intermediateValue))
                    {
                        continue;
                    }

                    intermediateValue.Append(element);
                    if (lastElement != ')')
                    {
                        lastElement = element;
                    }
                }
                else if (OperatorValues.ContainsKey(element))
                {
                    if (intermediateValue.Length != 0)
                    {
                        if (lastElement == ')')
                        {
                            RewriteExpr('*', stackOperations, resultRpn);
                        }

                        var newElement = GetElement(EType.Operand, intermediateValue.GetDouble());
                        resultRpn.Add(newElement);
                        intermediateValue.Clear();

                        if (element == '(')
                        {
                            RewriteExpr('*', stackOperations, resultRpn);
                            lastElement = '*';
                        }
                    }

                    if (element == '(' || element == ')')
                    {
                        countParentheses += CheckAsParentheses(element, countParentheses);
                    }

                    // If before the operator have another operator.
                    if (resultRpn.Count != 0)
                    {
                        if (OperatorValues.ContainsKey(lastElement))
                        {
                            switch (lastElement)
                            {
                                case '/':
                                case '*':
                                case '+':
                                case '-':
                                    if (element != '(')
                                    {
                                        throw new ArgumentException("Недопустимый ввод: " + lastElement + element);
                                    }

                                    break;
                                case '(':
                                    switch (element)
                                    {
                                        case '(':
                                            RewriteExpr('(', stackOperations, resultRpn);
                                            lastElement = '(';
                                            continue;
                                        case '-':
                                            intermediateValue.Append(element);
                                            continue;
                                        default:
                                            throw new ArgumentException("Недопустимый ввод: (" + element);
                                    }
                                case ')':
                                    if (element == '(')
                                    {
                                        RewriteExpr('*', stackOperations, resultRpn);
                                        RewriteExpr('(', stackOperations, resultRpn);
                                        lastElement = '(';
                                    }
                                    else
                                    {
                                        RewriteExpr(element, stackOperations, resultRpn);
                                        lastElement = element;
                                    }

                                    continue;
                            }
                        }
                    }

                    RewriteExpr(element, stackOperations, resultRpn);
                    lastElement = element;
                }
                else
                {
                    throw new ArgumentException("Неопознанный символ: " + element);
                }
            }

            if (countParentheses != 0)
            {
                throw new ArgumentException("Закрыты не все скобки");
            }

            if (intermediateValue.Length != 0)
            {
                if (lastElement == ')')
                {
                    RewriteExpr('*', stackOperations, resultRpn);
                }

                resultRpn.Add(GetElement(EType.Operand, intermediateValue.GetDouble()));
            }

            for (var i = 0; i < stackOperations.Count; i++)
            {
                resultRpn.Add(GetElement(EType.Operator, 0.0, stackOperations.Pop()));
            }

            return resultRpn;
        }

        /// <summary>
        /// Checks the operators in an expression and writes to resultRPN.
        /// </summary>
        /// <param name="operation">Operation of expression.</param>
        /// <param name="stackOperations">Stack with previous operations.</param>
        /// <param name="resultRpn">List containing a list of elements according to the RPN algoritm.</param>
        private static void RewriteExpr(char operation, Stack<char> stackOperations, List<Element> resultRpn)
        {
            // If stack is empty then operation from input string rewrite to stack
            // if another symbol from input string is "(" then he push to stack.
            if (stackOperations.Count == 0 || operation == '(')
            {
                stackOperations.Push(operation);
                return;
            }

            // If symbol is ")" then all operations from near "(" to ")" extrude from stack 
            // parentheses are not written but destroy.
            if (operation == ')')
            {
                for (var i = stackOperations.Count; i > 0; i--)
                {
                    if (stackOperations.Peek() == '(')
                    {
                        stackOperations.Pop();
                        return;
                    }
                    else
                    {
                        resultRpn.Add(GetElement(EType.Operator, 0.0, stackOperations.Pop()));
                    }
                }
            }

            // Operation extredes from stack all operations with larges or equals priority to output line.
            for (var i = stackOperations.Count; i >= 0; i--)
            {
                var stackOperation = GetStackOperation(stackOperations);
                if (stackOperations.Count > 0 && OperatorValues[operation] <= OperatorValues[stackOperation])
                {
                    resultRpn.Add(GetElement(EType.Operator, 0.0, stackOperations.Pop()));
                }
                else
                {
                    stackOperations.Push(operation);
                    return;
                }
            }
        }

        /// <summary>
        /// Return a separator according to user system parameters.
        /// </summary>
        /// <returns>Separator for expression.</returns>
        private static char FormatToCulture()
        {
            var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            return separator;
        }

        /// <summary>
        /// Checks last element in stack. If element is an mathematical operation translate to char.
        /// </summary>
        /// <param name="stack">Stack with set values.</param>
        /// <exception cref="ArgumentException">Throw when OperatorValues not contains stack end element.</exception>
        /// <returns>
        /// Math operation from the stack.
        /// </returns>
        private static char GetStackOperation(Stack<char> stack)
        {
            var stackOperation = ' ';
            if (stack.Count != 0)
            {
                stackOperation = stack.Peek();
                if (!OperatorValues.ContainsKey(stackOperation))
                {
                    throw new ArgumentException("Стек не содержит операцию");
                }
            }

            return stackOperation;
        }

        /// <summary>
        /// Check symbols in expensive on validate.
        /// </summary>
        /// <param name="exprText">Expression in the form of string.</param>
        /// <exception cref="ArgumentException">Throw when symbol do not stay on start in expensive.</exception>
        private static void ExpressionValidation(string exprText)
        {
            if (exprText.Length == 1 && OperatorValues.ContainsKey(exprText[0]))
            {
                throw new ArgumentException("Недопустимое выражение: " + exprText[0]);
            }

            if (OperatorValues.ContainsKey(exprText[^1]) && exprText[^1] != ')')
            {
                throw new ArgumentException("Недопустимый символ в конце выражения: " + exprText[^1]);
            }

            if (!AvailableSymbols.Contains(exprText[0]) && !OperandValues.Contains(exprText[0]))
            {
                throw new ArgumentException("Недопустимый символ вначале выражения");
            }
        }

        /// <summary>
        /// Check for a separator on the operand.
        /// </summary>
        /// <param name="element">Current element in expensive.</param>
        /// <param name="separator">Used separator.</param>
        /// <param name="intermediateValue">Intermediate value of operand.</param>
        /// <exception cref="ArgumentException">Throw when separator is first symbol in operand.</exception>
        /// <returns>
        /// True if operand do not contains separator.
        /// False if operand contains separator.
        /// </returns>
        private static bool CheckForOperands(char element, char separator, StringBuilder intermediateValue)
        {
            if (element != separator)
            {
                return true;
            }

            if (intermediateValue.Contains(separator))
            {
                return false;
            }

            if (intermediateValue.Equals(""))
            {
                throw new ArgumentException("Перед разделителем нет значения");
            }

            return true;
        }

        /// <summary>
        /// Increase or decrease the counter, which counts the number of parentheses.
        /// </summary>
        /// <param name="parentheses">Stack with set string values.</param>
        /// <param name="countParentheses">Stack with set string values.</param>
        /// <exception cref="ArgumentException">If there is no opener before the closing bracket.</exception>
        /// <returns>
        /// Type integer: 1 if element is opener bracket.
        /// 			  -1 if element is closer bracket.
        /// 			  Default 0.
        /// </returns>
        private static int CheckAsParentheses(char parentheses, int countParentheses)
        {
            switch (parentheses)
            {
                case '(':
                    return 1;
                case ')':
                    if ((countParentheses - 1) < 0)
                    {
                        throw new ArgumentException("Неверно поставлены скобки");
                    }

                    return -1;
            }

            return 0;
        }

        /// <summary>
        /// Create new Element from the transferred parameters.
        /// </summary>
        /// <param name="type">Type of element being created.</param>
        /// <param name="operand">Operand (if need) of element being created.</param>
        /// <param name="operation">Operation (if need) of element being created.</param>
        /// <exception cref="ArgumentException">Throw when input parameters are incompatible.</exception>
        /// <returns>
        /// New Element with transferred parameters.
        /// </returns>
        private static Element GetElement(EType type, double operand = 0.0, char operation = '0')
        {
            if (type == EType.Operator)
            {
                switch (operation)
                {
                    case '+':
                        return new Element {Type = EType.Operator, Operation = EOperation.Add, Operand = 0.0};
                    case '-':
                        return new Element {Type = EType.Operator, Operation = EOperation.Sub, Operand = 0.0};
                    case '*':
                        return new Element {Type = EType.Operator, Operation = EOperation.Mul, Operand = 0.0};
                    case '/':
                        return new Element {Type = EType.Operator, Operation = EOperation.Div, Operand = 0.0};
                }
            }
            else
            {
                return new Element {Type = EType.Operand, Operation = EOperation.None, Operand = operand};
            }

            throw new ArgumentException("Заданы неверные параметры: " + type + " - " + operation);
        }
    }

    public static class StringBuilderContains
    {
        public static bool Contains(this StringBuilder sb, char separator)
        {
            for (var i = 0; i < sb.Length; i++)
            {
                if (sb[i].Equals(separator))
                {
                    return true;
                }
            }

            return false;
        }

        public static double GetDouble(this StringBuilder sb)
        {
            if (double.TryParse(sb.ToString(), out var result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException(sb + " - не может быть преобразовано в тип double.");
            }
        }
    }
}
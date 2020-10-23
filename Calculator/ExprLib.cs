﻿using System;
using System.Collections.Generic;

namespace Calculator
{
    internal static class ExprLib
    {
        /// <summary>
        /// Get valid expression for reverse Polish notation. Calculate and return received value.
        /// </summary>
        /// <param name="inputExpression">User incoming expression.</param>
        /// <exception cref="ArgumentException">Throw when input expression is empty.</exception>
        /// <returns>
        /// Computed expression value.
        /// </returns>
        public static double PolandRevertMath(string inputExpression)
        {
            if (string.IsNullOrEmpty(inputExpression))
            {
                throw new ArgumentException("Ничего не введено!");
            }

            var expression = ExprConstructor.ExprToArr(inputExpression);
            var result = CalculateExpression(expression);
            return result.Operand;
        }

        /// <summary>
        /// Writes elements to the stack. Passes the two elements preceding the operator to the method, which evaluates the result.
        /// Iterations are repeated until a logical result.
        /// </summary>
        /// <param name="exprRpn">Expression written for calculation according to the algorithm.</param>
        /// <returns>
        /// Return calculate element.
        /// </returns>
        private static Element CalculateExpression(IEnumerable<Element> exprRpn)
        {
            var stack = new Stack<Element>();
            var operandDefault = new Element { Type = EType.Operand, Operation = EOperation.None, Operand = 0.0 };

            foreach (var element in exprRpn)
            {
                if (element.Type == EType.Operator)
                {
                    var rightOperand = stack.Pop();
                    var leftOperand = stack.Count > 0 ? stack.Pop() : operandDefault;
                    var result = ArithmeticOperations.PerformOperation(element, rightOperand, leftOperand);

                    stack.Push(result);
                }
                else
                {
                    stack.Push(element);
                }
            }
            return stack.Pop();
        }
    }

    internal struct Element
    {
        public EType Type;
        public EOperation Operation;
        public double Operand;
    }

    internal enum EType { Operator, Operand }

    internal enum EOperation { Mul, Div, Sub, Add, None }
}
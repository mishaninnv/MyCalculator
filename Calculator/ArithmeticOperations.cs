using System;
using System.Collections.Generic;

namespace Calculator
{
    internal static class ArithmeticOperations
    {
        private static readonly Dictionary<EOperation, Func<double, double, double>> Operations =
            new Dictionary<EOperation, Func<double, double, double>>
            {
                {EOperation.Add, (x, y) => x + y},
                {EOperation.Sub, (x, y) => x - y},
                {EOperation.Mul, (x, y) => x * y},
                {EOperation.Div, (x, y) => x / y}
            };

        /// <summary>
        /// Performs the action of addition, subtraction, division, multiplication.
        /// </summary>
        /// <param name="operation">Performed operation.</param>
        /// <param name="rightElement">Right operand in expression.</param>
        /// <param name="leftElement">Left operand in expression.</param>
        /// <exception cref="ArgumentException">
        /// Throw when transferred unacceptable operation. Not contained in Operations.
        /// Or if operation equals "/" and right operand equals 0. 
        /// </exception>
        /// <returns>
        /// The result of a given action on numbers.
        /// </returns>
        public static Element PerformOperation(Element operation, Element rightElement, Element leftElement)
        {
            if (!Operations.ContainsKey(operation.Operation))
            {
                throw new ArgumentException("Недопустимый оператор: " + operation);
            }

            if (operation.Operation == EOperation.Div && Math.Abs(rightElement.Operand) < double.Epsilon)
            {
                throw new ArgumentException("Нельзя делить на ноль");
            }

            var result = Operations[operation.Operation](leftElement.Operand, rightElement.Operand);

            return new Element {Type = EType.Operand, Operation = EOperation.None, Operand = result};
        }
    }
}
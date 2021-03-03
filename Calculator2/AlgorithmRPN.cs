using System;
using System.Collections.Generic;
using static Calculator2.MaterialLibrary;

namespace Calculator2
{
    public class AlgorithmRPN
    {
        private List<Element> _expressionToRpn = new List<Element>();
        private Stack<Element> _elementsToCalculate = new Stack<Element>();
        private Stack<Element> _intermediateOperators = new Stack<Element>();
        private Element _currElement;

        public string Result { get; private set; }

        public AlgorithmRPN(ExpressionHandler expressionHandler)
        {
            ConvertToRPN(expressionHandler.GetList);
            Calculate();
        }

        private void ConvertToRPN(List<Element> expression)
        {
            for (var i = 0; i < expression.Count; i++)
            {
                _currElement = expression[i];

                if (_currElement.Type == EType.Operand)
                {
                    _expressionToRpn.Add(_currElement);
                }
                else
                {
                    ActionOverOperator();
                }
            }

            AddingLatestOperators();
        }

        private void ActionOverOperator()
        {
            if (_intermediateOperators.Count <= 0)
            {
                _intermediateOperators.Push(_currElement);
            }
            else
            {
                AddOperatorInStack();
            }
        }

        private void AddOperatorInStack()
        {
            switch (_currElement.Value)
            {
                case "(":
                    _intermediateOperators.Push(_currElement);
                    break;
                case ")":
                    AddingOperatorsBetweenBounds();
                    break;
                default:
                    CheckPrioritiesOperators();
                    break;
            }
        }

        private void AddingOperatorsBetweenBounds()
        {
            while (_intermediateOperators.Count > 0)
            {
                var currOperator = _intermediateOperators.Peek();

                if (OperatorValues[currOperator.Value] < OperatorValues[_currElement.Value])
                {
                    _intermediateOperators.Pop();
                    return;
                }

                _expressionToRpn.Add(_intermediateOperators.Pop());
            }
        }

        private void CheckPrioritiesOperators()
        {
            while (_intermediateOperators.Count > 0)
            {
                var currOperator = _intermediateOperators.Peek();

                if (OperatorValues[currOperator.Value] < OperatorValues[_currElement.Value])
                {
                    _intermediateOperators.Push(_currElement);
                    return;
                }

                _expressionToRpn.Add(_intermediateOperators.Pop());
            }

            _intermediateOperators.Push(_currElement);
        }

        private void AddingLatestOperators()
        {
            while (_intermediateOperators.Count > 0)
            {
                _expressionToRpn.Add(_intermediateOperators.Pop());
            }
        }

        private void Calculate()
        {
            foreach (var Element in _expressionToRpn)
            {
                if (Element.Type == EType.Operator)
                {
                    var rightOperand = _elementsToCalculate.Pop();
                    var leftOperand = _elementsToCalculate.Pop();

                    if (Element.Value.Equals("/")) CheckRightOperandForZero(rightOperand);

                    var result = Operations[Element.Value](leftOperand.Value, rightOperand.Value);

                    _elementsToCalculate.Push(new Element {Type = EType.Operand, Value = result});
                }
                else
                {
                    _elementsToCalculate.Push(Element);
                }
            }

            Result = _elementsToCalculate.Pop().Value;
        }

        private void CheckRightOperandForZero(Element rightOperand)
        {
            if (rightOperand.Value.Equals("0"))
                throw new ArgumentException("Нельзя делить на ноль.");
        }
    }
}
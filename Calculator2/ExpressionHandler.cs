using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static Calculator2.MaterialLibrary;

namespace Calculator2
{
    public class ExpressionHandler
    {
        private StringBuilder _inputExpression;
        private List<Element> _expressionFromElements = new List<Element>();
        private StringBuilder _lastSymbols = new StringBuilder();
        private readonly char _separator;
        private int _boundsCounter;
        private int _currIndex;

        public List<Element> GetList => _expressionFromElements;

        public ExpressionHandler(string expression)
        {
            _inputExpression = new StringBuilder(expression);
            _separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            CheckValidExpression();
        }

        private void CheckValidExpression()
        {
            CheckEqualsNull();
            RemoveSpaces();
            CheckSeparator();
            CheckFirstSymbolValidity();
            CheckClosingBounds();
            CheckOtherParameters();
        }

        private void CheckEqualsNull()
        {
            if (string.IsNullOrEmpty(_inputExpression.ToString()))
                throw new ArgumentException("Ничего не введено.");
        }

        private void RemoveSpaces()
        {
            _inputExpression = _inputExpression.Replace(" ", "");
        }

        private void CheckSeparator()
        {
            _inputExpression = _separator.Equals('.')
                ? _inputExpression.Replace(',', '.')
                : _inputExpression.Replace('.', ',');
        }

        private void CheckFirstSymbolValidity()
        {
            if (!SymbolsToStart.Contains(_inputExpression[0]))
                throw new ArgumentException($"Недопустимый начальный символ: {_inputExpression[0]}");

            if (SpecialSymbolsToStart.Contains(_inputExpression[0]))
            {
                _inputExpression.Insert(0, 0);
            }
        }

        private void CheckClosingBounds()
        {
            var countLeftBounds = Regex.Matches(_inputExpression.ToString(), @"\(");
            var countRightBounds = Regex.Matches(_inputExpression.ToString(), @"\)");

            if (countLeftBounds.Count != countRightBounds.Count)
                throw new ArgumentException("Не все скобки закрыты.");
        }

        private void CheckOtherParameters()
        {
            for (var i = 0; i < _inputExpression.Length; i++)
            {
                _currIndex = i;
                CheckCorrectedSymbol();
                if (_inputExpression[i].Equals(_separator)) CheckSymbolBeforeSeparator();
                if (_inputExpression[i].Equals('(')) CheckSymbolsNearOpeningBracket();
                if (_inputExpression[i].Equals(')')) CheckSymbolsNearClosingBracket();
                CheckSymbolSequence();
                AddingSymbolToList();
            }
            
            CheckLastSymbol();
        }

        private void CheckCorrectedSymbol()
        {
            if (!ValidSymbols.Contains(_inputExpression[_currIndex]))
                throw new ArgumentException($"Недопустимый символ в выражении: {_inputExpression[_currIndex]}");
        }

        private void CheckSymbolBeforeSeparator()
        {
            var previousSymbol = _inputExpression[_currIndex - 1];

            if (!SymbolsBeforeSeparator.Contains(previousSymbol))
            {
                _inputExpression.Insert(_currIndex, 0);
            }
        }

        private void CheckSymbolsNearOpeningBracket()
        {
            _boundsCounter++;
            
            if (_currIndex > 0)
            {
                var previousElement = _inputExpression[_currIndex - 1];
                
                if (SymbolsBeforeOpenBound.Contains(previousElement))
                    _inputExpression.Insert(_currIndex, '*');
                
                if(previousElement.Equals(_separator))
                    throw new ArgumentException($"Недопустимая последовательность символов {previousElement} и {_inputExpression[_currIndex]}");
            }

            var nextElement = _inputExpression[_currIndex + 1];

            if (SpecialSymbolsAfterOpenBound.Contains(nextElement))
                _inputExpression.Insert(_currIndex + 1, 0);
            
            if(ProhibitedSymbolsAfterOpenBound.Contains(nextElement))
                throw new ArgumentException($"Недопустимая последовательность символов {_inputExpression[_currIndex]} и {nextElement}");
        }

        private void CheckSymbolsNearClosingBracket()
        {
            _boundsCounter--;
            
            if (_boundsCounter < 0)
                throw new ArgumentException($"Неверный порядок расстановки скобок {_inputExpression}");
            
            if (_inputExpression.Length > _currIndex + 1 && SpecialSymbolsAfterCloseBound.Contains(_inputExpression[_currIndex + 1]))
            {
                _inputExpression.Insert(_currIndex + 1, '*');
            }

            var previousElement = _inputExpression[_currIndex - 1];

            if(ProhibitedSymbolsBeforeCloseBound.Contains(previousElement))
                throw new ArgumentException($"Недопустимая последовательность символов {previousElement} и {_inputExpression[_currIndex]}");
        }

        private void CheckSymbolSequence()
        {
            if (ConsistensSymbols.Contains(_inputExpression[_currIndex]) &&
                ConsistensSymbols.Contains(_inputExpression[_currIndex - 1]))
                throw new ArgumentException(
                    $"Неверная последовательность элементов: {_inputExpression[_currIndex - 1]} и {_inputExpression[_currIndex]}");
        }
        
        private void AddingSymbolToList()
        {
            if (!OperatorValues.ContainsKey(_inputExpression[_currIndex].ToString()))
            {
                ActionsOverOperand();
            }
            else
            {
                ActionsOverOperator();
            }
        }

        private void ActionsOverOperand()
        {
            _lastSymbols.Append(_inputExpression[_currIndex]);
        }

        private void ActionsOverOperator()
        {
            CheckLastSymbol();

            var newOperator = GetElement(EType.Operator, _inputExpression[_currIndex].ToString());
            _expressionFromElements.Add(newOperator);
        }

        private void CheckLastSymbol()
        {
            if (_lastSymbols.Length > 0) AddOperand();
        }
        
        private void AddOperand()
        {
            var newOperand = GetElement(EType.Operand, _lastSymbols.ToString());
            _expressionFromElements.Add(newOperand);
            _lastSymbols.Clear();
        }

        private Element GetElement(EType type, string value)
        {
            return new Element {Type = type, Value = value};
        }
    }
}
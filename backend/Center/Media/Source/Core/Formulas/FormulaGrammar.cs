// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaGrammar.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Defines the grammar for formulas.
    /// </summary>
    public class FormulaGrammar : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaGrammar"/> class.
        /// </summary>
        public FormulaGrammar() : base(caseSensitive: false)
        {
            // Terminals
            var identifier = new IdentifierTerminal("Identifier");
            var number = new NumberLiteral("Number", NumberOptions.IntOnly | NumberOptions.AllowSign);
            var stringLiteral = new StringLiteral("string", "\"", StringOptions.NoEscapes);
            stringLiteral.AddStartEnd("'", "'", StringOptions.NoEscapes | StringOptions.AllowsDoubledQuote);

            // Non-terminals
            var formula = new NonTerminal("Formula", typeof(StatementListNode));
            var expression = new NonTerminal("Expression");
            var simpleExpression = new NonTerminal("SimpleExpression");
            var binaryExpression = new NonTerminal("BinaryExpression", typeof(BinaryExpressionNode));
            var binaryOperator = new NonTerminal("BinaryOperator", "operator");
            var method = new NonTerminal("Method", typeof(MethodCallNode));
            var variable = new NonTerminal("variable", typeof(GenericNode));
            var optionalNumber = new NonTerminal("optionalNumber");
            var optionalIdentifier = new NonTerminal("optionalIdentifier");
            var optionalExpression = new NonTerminal("optionalExpression", typeof(ParamListNode));
            var switchCases = new NonTerminal("optionalSwitchExpression", typeof(ExpressionListNode));
            var starExpression = new NonTerminal("starExpression", typeof(StarExpressionNode));
            var starExpressionList = new NonTerminal("starExpressionList", typeof(ExpressionListNode));
            var switchMethod = new NonTerminal("switchMethod", typeof(SwitchCallNode));
            var csvMappingMethod = new NonTerminal("csvMappingMethod", typeof(CsvMappingCallNode));
            var switchMethodWithoutCases = new NonTerminal("switchMethodWithoutCases", typeof(SwitchCallNode));
            var switchCase = new NonTerminal("switchCase", typeof(BinaryExpressionNode));
            var switchValue = new NonTerminal("switchValue");

            // Rules
            var parameterSeparator = this.ToTerm(";");

            formula.Rule = this.ToTerm("=") + expression;
            optionalNumber.Rule = this.ToTerm("[") + number + this.ToTerm("]");
            optionalIdentifier.Rule = this.ToTerm("{") + identifier + this.ToTerm("}");
            starExpression.Rule = parameterSeparator + expression;
            starExpressionList.Rule = this.MakeStarRule(starExpressionList, starExpression);
            optionalExpression.Rule = (expression + starExpressionList) | this.Empty;
            switchCases.Rule = this.MakeListRule(switchCases, parameterSeparator, switchCase);
            binaryExpression.Rule = simpleExpression + binaryOperator + simpleExpression;
            expression.Rule = simpleExpression | binaryExpression | csvMappingMethod | switchMethod
                              | switchMethodWithoutCases;

            simpleExpression.Rule = method | variable | stringLiteral | number;
            variable.Rule = this.ToTerm("$") + identifier + this.ToTerm(".") + identifier + optionalNumber.Q()
                            + optionalIdentifier.Q();
            method.Rule = identifier + this.ToTerm("(") + optionalExpression + this.ToTerm(")");
            switchMethod.Rule = this.ToTerm("SWITCH") + this.ToTerm("(") + variable + parameterSeparator
                                + switchCases + parameterSeparator + expression + this.ToTerm(")");
            switchMethodWithoutCases.Rule = this.ToTerm("SWITCH") + this.ToTerm("(") + variable + parameterSeparator
                                            + expression + this.ToTerm(")");

            csvMappingMethod.Rule = this.ToTerm("CSVMAPPING") + this.ToTerm("(") + expression + parameterSeparator
                                    + expression + parameterSeparator + expression + parameterSeparator + switchCases
                                    + this.ToTerm(")");
            switchCase.Rule = switchValue + this.ToTerm(":") + expression;
            switchValue.Rule = stringLiteral | number;
            binaryOperator.Rule = this.ToTerm(BinaryOperation.IsEqual.Operator) | BinaryOperation.NotEquals.Operator
                                  | BinaryOperation.GreaterThan.Operator | BinaryOperation.GreaterThanOrEqual.Operator
                                  | BinaryOperation.LessThan.Operator | BinaryOperation.LessThanOrEqual.Operator;

            this.Root = formula;

            this.MarkTransient(expression, simpleExpression, switchValue);

            // Operators
            this.RegisterOperators(
                10,
                BinaryOperation.IsEqual.Operator,
                BinaryOperation.NotEquals.Operator,
                BinaryOperation.GreaterThan.Operator,
                BinaryOperation.GreaterThanOrEqual.Operator,
                BinaryOperation.LessThan.Operator,
                BinaryOperation.LessThanOrEqual.Operator);

            // Automatically add NewLine before EOF so that the BNF rules work correctly when there's no final
            // line break in source
            this.LanguageFlags =
                LanguageFlags.NewLineBeforeEOF | LanguageFlags.CreateAst | LanguageFlags.SupportsBigInt;
        }

        /// <summary>
        /// The handler to build the AST
        /// </summary>
        /// <param name="language">the language</param>
        /// <param name="parseTree">the parse tree</param>
        public override void BuildAst(LanguageData language, ParseTree parseTree)
        {
            if (!LanguageFlags.IsSet(LanguageFlags.CreateAst))
            {
                return;
            }

            var astContext = new AstContext(language)
            {
                DefaultLiteralNodeType = typeof(LiteralValueNode),
                DefaultIdentifierNodeType = typeof(IdentifierNode),
                DefaultNodeType = typeof(AstNode)
            };

            var astBuilder = new AstBuilder(astContext);
            astBuilder.BuildAst(parseTree);
        }
    }
}

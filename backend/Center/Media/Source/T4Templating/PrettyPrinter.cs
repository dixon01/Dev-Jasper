// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrettyPrinter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PrettyPrinter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;

    using ICSharpCode.NRefactory.CSharp;

    /// <summary>
    /// Static class that allows to clean up the generated contents of a T4 template.
    /// </summary>
    public static class PrettyPrinter
    {
        /// <summary>
        /// Cleans up the given string builder.
        /// Use as follows as the last line of the main body of the template:
        /// <code>
        /// PrettyPrinter.CleanUp(this.GenerationEnvironment);
        /// </code>
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public static void CleanUp(StringBuilder content)
        {
            var code = content.ToString();
            content.Clear();

            var parser = new CSharpParser();
            var tree = parser.Parse(code);

            var codeReorderVisitor = new CodeReorderVisitor();
            codeReorderVisitor.VisitSyntaxTree(tree);

            var writer = new System.IO.StringWriter(content);
            var policy = CreateFormattingOptions();
            var outputVisitor = new CSharpOutputVisitor(new GorbaOutputFormatter(writer), policy);
            outputVisitor.VisitSyntaxTree(tree);
        }

        /// <summary>
        /// Creates formatting options that are as close as possible to StyleCop.
        /// </summary>
        /// <returns>
        /// The new <see cref="CSharpFormattingOptions"/>.
        /// </returns>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Lot of property setters needed.")]
        private static CSharpFormattingOptions CreateFormattingOptions()
        {
            var options = FormattingOptionsFactory.CreateEmpty();
            options.IndentNamespaceBody = true;
            options.IndentClassBody = true;
            options.IndentInterfaceBody = true;
            options.IndentStructBody = true;
            options.IndentEnumBody = true;
            options.IndentMethodBody = true;
            options.IndentPropertyBody = true;
            options.IndentEventBody = true;
            options.IndentBlocks = true;
            options.IndentSwitchBody = true;
            options.IndentCaseBody = true;
            options.IndentBreakStatements = false;
            options.NamespaceBraceStyle = BraceStyle.NextLine;
            options.ClassBraceStyle = BraceStyle.NextLine;
            options.InterfaceBraceStyle = BraceStyle.NextLine;
            options.StructBraceStyle = BraceStyle.NextLine;
            options.EnumBraceStyle = BraceStyle.NextLine;
            options.MethodBraceStyle = BraceStyle.NextLine;
            options.ConstructorBraceStyle = BraceStyle.NextLine;
            options.DestructorBraceStyle = BraceStyle.NextLine;
            options.AnonymousMethodBraceStyle = BraceStyle.EndOfLine;
            options.PropertyBraceStyle = BraceStyle.NextLine;
            options.PropertyGetBraceStyle = BraceStyle.NextLine;
            options.PropertySetBraceStyle = BraceStyle.NextLine;
            options.AllowPropertyGetBlockInline = true;
            options.AllowPropertySetBlockInline = true;
            options.EventBraceStyle = BraceStyle.NextLine;
            options.EventAddBraceStyle = BraceStyle.NextLine;
            options.EventRemoveBraceStyle = BraceStyle.NextLine;
            options.AllowEventAddBlockInline = true;
            options.AllowEventRemoveBlockInline = true;
            options.StatementBraceStyle = BraceStyle.NextLine;
            options.ElseNewLinePlacement = NewLinePlacement.NewLine;
            options.CatchNewLinePlacement = NewLinePlacement.NewLine;
            options.FinallyNewLinePlacement = NewLinePlacement.NewLine;
            options.WhileNewLinePlacement = NewLinePlacement.NewLine;
            options.ArrayInitializerWrapping = Wrapping.WrapIfTooLong;
            options.ArrayInitializerBraceStyle = BraceStyle.NextLine;
            options.SpaceBeforeMethodCallParentheses = false;
            options.SpaceBeforeMethodDeclarationParentheses = false;
            options.SpaceBeforeConstructorDeclarationParentheses = false;
            options.SpaceBeforeDelegateDeclarationParentheses = false;
            options.SpaceAfterMethodCallParameterComma = true;
            options.SpaceAfterConstructorDeclarationParameterComma = true;
            options.SpaceBeforeNewParentheses = false;
            options.SpacesWithinNewParentheses = false;
            options.SpacesBetweenEmptyNewParentheses = false;
            options.SpaceBeforeNewParameterComma = false;
            options.SpaceAfterNewParameterComma = true;
            options.SpaceBeforeIfParentheses = true;
            options.SpaceBeforeWhileParentheses = true;
            options.SpaceBeforeForParentheses = true;
            options.SpaceBeforeForeachParentheses = true;
            options.SpaceBeforeCatchParentheses = true;
            options.SpaceBeforeSwitchParentheses = true;
            options.SpaceBeforeLockParentheses = true;
            options.SpaceBeforeUsingParentheses = true;
            options.SpaceAroundAssignment = true;
            options.SpaceAroundLogicalOperator = true;
            options.SpaceAroundEqualityOperator = true;
            options.SpaceAroundRelationalOperator = true;
            options.SpaceAroundBitwiseOperator = true;
            options.SpaceAroundAdditiveOperator = true;
            options.SpaceAroundMultiplicativeOperator = true;
            options.SpaceAroundShiftOperator = true;
            options.SpaceAroundNullCoalescingOperator = true;
            options.SpacesWithinParentheses = false;
            options.SpaceWithinMethodCallParentheses = false;
            options.SpaceWithinMethodDeclarationParentheses = false;
            options.SpacesWithinIfParentheses = false;
            options.SpacesWithinWhileParentheses = false;
            options.SpacesWithinForParentheses = false;
            options.SpacesWithinForeachParentheses = false;
            options.SpacesWithinCatchParentheses = false;
            options.SpacesWithinSwitchParentheses = false;
            options.SpacesWithinLockParentheses = false;
            options.SpacesWithinUsingParentheses = false;
            options.SpacesWithinCastParentheses = false;
            options.SpacesWithinSizeOfParentheses = false;
            options.SpacesWithinTypeOfParentheses = false;
            options.SpacesWithinCheckedExpressionParantheses = false;
            options.SpaceBeforeConditionalOperatorCondition = true;
            options.SpaceAfterConditionalOperatorCondition = true;
            options.SpaceBeforeConditionalOperatorSeparator = true;
            options.SpaceAfterConditionalOperatorSeparator = true;
            options.SpacesWithinBrackets = false;
            options.SpacesBeforeBrackets = false;
            options.SpaceBeforeBracketComma = false;
            options.SpaceAfterBracketComma = true;
            options.SpaceBeforeForSemicolon = false;
            options.SpaceAfterForSemicolon = true;
            options.SpaceAfterTypecast = false;
            options.AlignEmbeddedIfStatements = true;
            options.AlignEmbeddedUsingStatements = true;
            options.PropertyFormatting = PropertyFormatting.AllowOneLine;
            options.SpaceBeforeMethodDeclarationParameterComma = false;
            options.SpaceAfterMethodDeclarationParameterComma = true;
            options.SpaceAfterDelegateDeclarationParameterComma = true;
            options.SpaceBeforeFieldDeclarationComma = false;
            options.SpaceAfterFieldDeclarationComma = true;
            options.SpaceBeforeLocalVariableDeclarationComma = false;
            options.SpaceAfterLocalVariableDeclarationComma = true;
            options.SpaceBeforeIndexerDeclarationBracket = false;
            options.SpaceWithinIndexerDeclarationBracket = false;
            options.SpaceBeforeIndexerDeclarationParameterComma = false;
            options.SpaceInNamedArgumentAfterDoubleColon = true;
            options.SpaceAfterIndexerDeclarationParameterComma = true;
            options.BlankLinesBeforeUsings = 0;
            options.BlankLinesAfterUsings = 1;
            options.BlankLinesBeforeFirstDeclaration = 0;
            options.BlankLinesBetweenTypes = 1;
            options.BlankLinesBetweenFields = 0;
            options.BlankLinesBetweenEventFields = 0;
            options.BlankLinesBetweenMembers = 1;
            options.KeepCommentsAtFirstColumn = true;
            options.ChainedMethodCallWrapping = Wrapping.DoNotChange;
            options.MethodCallArgumentWrapping = Wrapping.DoNotChange;
            options.NewLineAferMethodCallOpenParentheses = true;
            options.MethodCallClosingParenthesesOnNewLine = true;
            options.IndexerArgumentWrapping = Wrapping.DoNotChange;
            options.NewLineAferIndexerOpenBracket = false;
            options.IndexerClosingBracketOnNewLine = false;
            options.IfElseBraceForcement = BraceForcement.AddBraces;
            options.ForBraceForcement = BraceForcement.AddBraces;
            options.ForEachBraceForcement = BraceForcement.AddBraces;
            options.WhileBraceForcement = BraceForcement.AddBraces;
            options.UsingBraceForcement = BraceForcement.AddBraces;
            options.FixedBraceForcement = BraceForcement.AddBraces;
            return options;
        }

        /// <summary>
        /// Orders all members of a type declaration correctly
        /// </summary>
        private class CodeReorderVisitor : DepthFirstAstVisitor
        {
            public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
            {
                typeDeclaration.Members.ReplaceWith(typeDeclaration.Members.OrderBy(e => e, new MemberComparer()));
                base.VisitTypeDeclaration(typeDeclaration);
            }
        }

        /// <summary>
        /// Compares members according to the StyleCop rules
        /// </summary>
        private class MemberComparer : IComparer<EntityDeclaration>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than,
            /// equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>,
            /// as shown in the following table.
            /// - Less than zero: <paramref name="x"/> is less than <paramref name="y"/>.
            /// - Zero: <paramref name="x"/> equals <paramref name="y"/>.
            /// - Greater than zero: <paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <param name="x">
            /// The first object to compare.
            /// </param>
            /// <param name="y">
            /// The second object to compare.
            /// </param>
            public int Compare(EntityDeclaration x, EntityDeclaration y)
            {
                // according to http://stylecop.codeplex.com/discussions/355722
                var compare = GetElementTypeIndex(x).CompareTo(GetElementTypeIndex(y));
                if (compare != 0)
                {
                    return compare;
                }

                compare = GetAccessModifierIndex(x).CompareTo(GetAccessModifierIndex(y));
                if (compare != 0)
                {
                    return compare;
                }

                compare = GetInstanceModifierIndex(x).CompareTo(GetInstanceModifierIndex(y));
                return compare;
            }

            /* according to http://stylecop.codeplex.com/discussions/355722:
             * File = 0,
             * Root = 1,
             * ExternAliasDirective = 2,
             * UsingDirective = 3,
             * AssemblyAttribute = 4,
             * Namespace = 5,
             * Field = 6,
             * Constructor = 7,
             * Destructor = 8,
             * Delegate = 9,
             * Event = 10,
             * Enum = 11,
             * Interface = 12,
             * Property = 13,
             * Accessor = 14,
             * Indexer = 15,
             * Method = 16,
             * Struct = 17,
             * Class = 18,
             * EnumItem = 19,
             * ConstructorInitializer = 20,
             * EmptyElement = 21
             */
            private static int GetElementTypeIndex(EntityDeclaration entity)
            {
                if (entity is FieldDeclaration)
                {
                    return 6;
                }

                if (entity is ConstructorDeclaration)
                {
                    return 7;
                }

                if (entity is DestructorDeclaration)
                {
                    return 8;
                }

                if (entity is DelegateDeclaration)
                {
                    return 9;
                }

                if (entity is EventDeclaration)
                {
                    return 10;
                }

                var type = entity as TypeDeclaration;
                if (type != null)
                {
                    switch (type.ClassType)
                    {
                        case ClassType.Enum:
                            return 11;
                        case ClassType.Interface:
                            return 12;
                        case ClassType.Struct:
                            return 17;
                        case ClassType.Class:
                            return 18;
                    }
                }

                if (entity is PropertyDeclaration)
                {
                    return 13;
                }

                if (entity is Accessor)
                {
                    return 14;
                }

                if (entity is IndexerDeclaration)
                {
                    return 15;
                }

                if (entity is MethodDeclaration || entity is OperatorDeclaration)
                {
                    return 16;
                }

                if (entity is EnumMemberDeclaration)
                {
                    return 19;
                }

                // we are missing ConstructorInitializer (20) and EmptyElement (21)
                // unknown:
                return 1000;
            }

            /* according to http://stylecop.codeplex.com/discussions/355722:
             * Public = 0, Internal = 1, ProtectedInternal = 2, Protected = 3, Private = 4, ProtectedAndInternal = 5
             */
            private static int GetAccessModifierIndex(EntityDeclaration entity)
            {
                if (entity.Modifiers.HasFlag(Modifiers.Public))
                {
                    return 0;
                }

                if (entity.Modifiers.HasFlag(Modifiers.Internal))
                {
                    return !entity.Modifiers.HasFlag(Modifiers.Protected) ? 1 : 2;
                }

                if (entity.Modifiers.HasFlag(Modifiers.Protected))
                {
                    return 3;
                }

                if (entity.Modifiers.HasFlag(Modifiers.Private))
                {
                    return 4;
                }

                // ProtectedAndInternal??? or unknown
                return 1000;
            }

            /* according to http://stylecop.codeplex.com/discussions/355722:
             * const = 0
             * static readonly = 1
             * static non-readonly = 2
             * instance readonly = 3
             * instance non-readonly = 4
             */
            private static int GetInstanceModifierIndex(EntityDeclaration entity)
            {
                if (entity.Modifiers.HasFlag(Modifiers.Const))
                {
                    return 0;
                }

                if (entity.Modifiers.HasFlag(Modifiers.Static))
                {
                    return entity.Modifiers.HasFlag(Modifiers.Readonly) ? 1 : 2;
                }

                return entity.Modifiers.HasFlag(Modifiers.Readonly) ? 3 : 4;
            }
        }

        /// <summary>
        /// Formats a little bit more like StyleCop (or ReSharper). This method is far from complete.
        /// </summary>
        private class GorbaOutputFormatter : TextWriterOutputFormatter
        {
            public GorbaOutputFormatter(TextWriter textWriter)
                : base(textWriter)
            {
                this.IndentationString = new string(' ', 4);
            }

            public override void StartNode(AstNode node)
            {
                if (node is ConstructorInitializer)
                {
                    this.Indentation++;
                    this.NewLine();
                }

                base.StartNode(node);
            }

            public override void EndNode(AstNode node)
            {
                if (node is ConstructorInitializer)
                {
                    this.Indentation--;
                }
                else if (node is EntityDeclaration
                    && !(node.Role == IndexerDeclaration.GetterRole || node.Role == IndexerDeclaration.SetterRole))
                {
                    this.NewLine();
                }
                else if (node is IfElseStatement
                    || node is SwitchStatement
                    || node is CheckedStatement
                    || node is DoWhileStatement
                    || node is FixedStatement
                    || node is ForStatement
                    || node is ForeachStatement
                    || node is LockStatement
                    || node is TryCatchStatement
                    || node is UncheckedStatement
                    || node is UnsafeStatement
                    || node is WhileStatement)
                {
                    this.NewLine();
                }

                base.EndNode(node);
            }
        }
    }
}

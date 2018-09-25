// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaTestBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class to provide a mockup for the FormualEditor features.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.Formulas;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Protocols.Ximple.Generic;

    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Base class to mock a dictionary.
    /// </summary>
    public class FormulaTestBase
    {
        /// <summary>
        /// Gets or sets the dictionary model to use.
        /// </summary>
        public DictionaryDataViewModel DictionaryDataViewModel { get; set; }

        /// <summary>
        /// Gets or sets the media shell to use.
        /// </summary>
        public Mock<IMediaShell> ShellMock { get; set; }

        /// <summary>
        /// Parses a given formula into a model.
        /// </summary>
        /// <param name="formula">The formula</param>
        /// <returns>The model</returns>
        protected EvalDataViewModelBase ParseFormula(string formula)
        {
            var grammar = new FormulaGrammar();
            var language = new LanguageData(grammar);
            var parser = new Parser(language);
            var parseTree = parser.Parse(formula);

            if (parseTree.HasErrors())
            {
                var messages = parseTree.ParserMessages.Select(pm => pm.Message);
                throw new InvalidOperationException(string.Join("\n", messages));
            }

            Assert.IsNotNull(parseTree.Root);
            Assert.IsNotNull(parseTree.Root.AstNode);

            var generator = new FormulaModelGenerator(this.ShellMock.Object);
            var model = generator.Generate((AstNode)parseTree.Root.AstNode);

            return model;
        }

        /// <summary>
        /// Creates an IMediaShell mock which contains a dictionary.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <returns>
        /// The media shell mock.
        /// </returns>
        protected Mock<IMediaShell> CreateMediaShell(DictionaryDataViewModel dictionary)
        {
            var mock = new Mock<IMediaShell>();
            mock.SetupGet(s => s.Dictionary).Returns(dictionary);
            return mock;
        }

        /// <summary>
        /// Creates a test dictionary.
        /// </summary>
        /// <returns>
        /// The dictionary.
        /// </returns>
        protected Dictionary GetDictionarySampleData()
        {
            return new Dictionary
                       {
                           Languages = new List<Language> { new Language { Index = 1, Name = "de" } },
                           Tables =
                               new List<Table>
                                   {
                                       new Table
                                           {
                                               Name = "Stops",
                                               Index = 1,
                                               MultiRow = true,
                                               MultiLanguage = true,
                                               Columns =
                                                   new List<Column>
                                                       {
                                                           new Column
                                                               {
                                                                   Name = "StopName",
                                                                   Index = 1,
                                                               }
                                                       }
                                           },
                                       new Table
                                           {
                                               Name = "Stops2",
                                               Index = 2,
                                               MultiRow = true,
                                               MultiLanguage = false,
                                               Columns =
                                                   new List<Column>
                                                       {
                                                           new Column
                                                               {
                                                                   Name = "StopName",
                                                                   Index = 1,
                                                               }
                                                       }
                                           },
                                       new Table
                                           {
                                               Name = "Stops3",
                                               Index = 3,
                                               MultiRow = false,
                                               MultiLanguage = false,
                                               Columns =
                                                   new List<Column>
                                                       {
                                                           new Column
                                                               {
                                                                   Name = "StopName",
                                                                   Index = 1,
                                                               }
                                                       }
                                           }
                                   }
                       };
        }
    }
}
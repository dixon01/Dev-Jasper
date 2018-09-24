namespace Luminator.MultiCast.BroadCast
{
    using System;
    using System.Text;

    public class RandomText
    {
        #region Static Fields

        private static readonly Random _random = new Random();

        private static string[] _words = { "anemone", "wagstaff", "man", "the", "for", "and", "a", "with", "bird", "fox" };

        #endregion

        #region Fields

        private readonly StringBuilder _builder;

        #endregion

        #region Constructors and Destructors

        public RandomText(string[] wordsIn = null)
        {
            if (wordsIn != null)
            {
                _words = wordsIn;
            }

            this._builder = new StringBuilder();
        }

        #endregion

        #region Public Properties

        public string Content
        {
            get
            {
                return this._builder.ToString();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void AddContentParagraphs(int numberParagraphs, int minSentences, int maxSentences, int minWords, int maxWords)
        {
            for (var i = 0; i < numberParagraphs; i++)
            {
                this.AddParagraph(_random.Next(minSentences, maxSentences + 1), minWords, maxWords);
                this._builder.Append("\n\n");
            }
        }

        #endregion

        #region Methods

        private void AddParagraph(int numberSentences, int minWords, int maxWords)
        {
            for (var i = 0; i < numberSentences; i++)
            {
                var count = _random.Next(minWords, maxWords + 1);
                this.AddSentence(count);
            }
        }

        private void AddSentence(int numberWords)
        {
            var b = new StringBuilder();
            // Add n words together.
            for (var i = 0; i < numberWords; i++) // Number of words
            {
                b.Append(_words[_random.Next(_words.Length)]).Append(" ");
            }
            var sentence = b.ToString().Trim() + ". ";
            // Uppercase sentence
            sentence = char.ToUpper(sentence[0]) + sentence.Substring(1);
            // Add this sentence to the class
            this._builder.Append(sentence);
        }

        #endregion
    }
}
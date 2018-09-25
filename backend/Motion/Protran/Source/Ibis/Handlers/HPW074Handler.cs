// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HPW074Handler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HPW074Handler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System.IO;
    using System.Text;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Csv;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Handler for HPW074 telegrams which looks up the sent index
    /// in the configured CSV file.
    /// </summary>
    public class HPW074Handler : TelegramHandler<HPW074>
    {
        private HPW074Config config;

        private string filePath;

        private GenericUsageHandler usage;

        /// <summary>
        /// Initializes a new instance of the <see cref="HPW074Handler"/> class.
        /// </summary>
        public HPW074Handler()
            : base(10)
        {
        }

        /// <summary>
        /// Configures this handler with the given telegram config and
        /// generic view dictionary.
        /// </summary>
        /// <param name="telegramConfig">
        /// The telegram config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public override void Configure(TelegramConfig telegramConfig, IIbisConfigContext configContext)
        {
            base.Configure(telegramConfig, configContext);

            this.config = (HPW074Config)telegramConfig;
            this.filePath = configContext.GetAbsolutePathRelatedToConfig(this.config.SpecialTextFile);
            this.usage = new GenericUsageHandler(telegramConfig.UsedFor, configContext.Dictionary);
        }

        /// <summary>
        /// Handles the telegram and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        protected override void HandleInput(HPW074 telegram)
        {
            var text = this.GetSpecialText(telegram.Data);
            var ximple = new Ximple();
            this.usage.AddCell(ximple, text);
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private string GetSpecialText(int index)
        {
            if (index == 0)
            {
                return string.Empty;
            }

            if (!File.Exists(this.filePath))
            {
                throw new FileNotFoundException("Special text reference file not found: " + this.filePath);
            }

            using (var reader = new CsvReader(this.filePath, Encoding.GetEncoding(this.config.Encoding)))
            {
                string[] line;
                while ((line = reader.GetCsvLine()) != null)
                {
                    if (line.Length < 2)
                    {
                        continue;
                    }

                    int readIndex;
                    if (ParserUtil.TryParse(line[0], out readIndex) && readIndex == index)
                    {
                        return string.Join("[br]", line, 1, line.Length - 1);
                    }
                }
            }

            return string.Empty;
        }
    }
}

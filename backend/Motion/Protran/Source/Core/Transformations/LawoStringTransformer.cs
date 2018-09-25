// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LawoStringTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LawoStringTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System.Text;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// LAWO string escape transformer.
    /// Modifies input bytes with offsets according to predefined rules.
    /// </summary>
    public class LawoStringTransformer : Transformer<byte[], string, LawoString>
    {
        private Encoding encoding;

        /// <summary>
        /// Configures this transformer with the given configuration.
        /// </summary>
        /// <param name="config">
        /// The configuration.
        /// </param>
        protected override void Configure(LawoString config)
        {
            base.Configure(config);

            this.encoding = Encoding.GetEncoding(config.CodePage);
        }

        /// <summary>
        /// Actual transformation method to be implemented by subclasses.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override string DoTransform(byte[] value)
        {
            // this code is a close translation of the existing TFTPlayer code
            // we keep it like this for compatibility reasons
            bool add60 = false;
            bool add60Once = false;
            bool addA0 = false;
            bool addA0Once = false;
            bool lowerCase = false;

            int outputPos = 0;
            for (int inputPos = 0; inputPos < value.Length; inputPos++)
            {
                switch (value[inputPos])
                {
                    case 0x01:
                        add60Once = true;
                        continue;
                    case 0x0B:
                        add60 = !add60;
                        continue;
                    case 0x02:
                        addA0Once = true;
                        continue;
                    case 0x0C:
                        addA0 = !addA0;
                        continue;
                    case 0x06:
                        lowerCase = !lowerCase;
                        continue;
                    case 0x04:
                    case 0x05:
                        add60 = false;
                        add60Once = false;
                        addA0 = false;
                        addA0Once = false;
                        lowerCase = false;
                        break;
                }

                var input = value[inputPos];
                var output = input;
                if (add60)
                {
                    output = (byte)(input + 0x60);
                    add60Once = false;
                    addA0 = false;
                    addA0Once = false;
                }
                else if (add60Once)
                {
                    output = (byte)(input + 0x60);
                    add60Once = false;
                    addA0 = false;
                    addA0Once = false;
                }
                else if (addA0)
                {
                    output = (byte)(input + 0xA0);
                    addA0Once = false;
                }
                else if (addA0Once)
                {
                    output = (byte)(input + 0xA0);
                    addA0Once = false;
                }
                else if (lowerCase)
                {
                    if ((input >= 'A' && input <= 'Z') || input == 'Ä' || input == 'Ö' || input == 'Ü')
                    {
                        output = (byte)(input + 0x20);
                    }
                }

                value[outputPos++] = output;
            }

            return this.encoding.GetString(value, 0, outputPos);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainUnitConfigurationSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Qube.Configuration
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Utils;
    using Gorba.Common.Configuration.EPaper.MainUnit;

    /// <summary>
    /// The configuration serializer for a main unit.
    /// </summary>
    public class MainUnitConfigurationSerializer : IConfigurationSerializer
    {
        /// <summary>
        /// Serializes the given configuration into the binary format understood by units.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/> containing the binary serialized configuration. The <see cref="Stream.Position"/>
        /// is reset to 0;
        /// </returns>
        public Task<Stream> SerializeAsync(MainUnitConfig configuration)
        {
            var serializer = new ConfigurationSerializer(configuration);
            return serializer.SerializeAsync();
        }

        /// <summary>
        /// Differently than the <see cref="MainUnitConfigurationSerializer"/>, this implementation has a state (the
        /// <see cref="output"/> stream). A <see cref="ConfigurationSerializer"/> can only be used once (while the
        /// <see cref="MainUnitConfigurationSerializer"/> can be reused).
        /// </summary>
        private class ConfigurationSerializer
        {
            private const int MaxDisplays = 6;

            /// <summary>
            /// The value of the xxHash64 for an empty file.
            /// </summary>
            private const string EmptyFileHash64 = "99e9d85137db46ef";

            private readonly MainUnitConfig configuration;

            private readonly Stream output = new MemoryStream();

            public ConfigurationSerializer(MainUnitConfig configuration)
            {
                if (configuration == null)
                {
                    throw new ArgumentNullException("configuration");
                }

                this.configuration = configuration;
            }

            /// <summary>
            /// Serializes the configuration.
            /// </summary>
            /// <returns>
            /// The <see cref="Stream"/> containing the binary serialized version of the configuration.
            /// The <see cref="Stream.Position"/> value is reset to 0.
            /// </returns>
            public async Task<Stream> SerializeAsync()
            {
                await this.WriteMemorySectionAsync();
                await this.WriteOtherFilesSectionAsync();
                await this.WriteSchedulerTableSectionAsync();

                this.output.Position = 0;
                return this.output;
            }

            private static byte[] HexStringToByteArray(string hexString)
            {
                if (string.IsNullOrEmpty(hexString) || hexString.Length % 2 != 0)
                {
                    throw new ArgumentException("Invalid hex string");
                }

                var array = new byte[hexString.Length / 2];
                for (var i = 0; i < hexString.Length / 2; i++)
                {
                    array[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }

                return array;
            }

            private async Task WriteMemorySectionAsync()
            {
                await this.WriteSectionHeaderAsync(SectionType.MemorySlots, SectionVersion.Version00, 72);
                var memoryStream = new MemoryStream();
                await this.WriteHashAsync(memoryStream, EmptyFileHash64);
                await this.WriteHashAsync(memoryStream, EmptyFileHash64);
                await this.WriteHashAsync(memoryStream, EmptyFileHash64);
                for (var i = 0; i < MaxDisplays; i++)
                {
                    if (i >= this.configuration.DisplayUnits.Count)
                    {
                        await this.WriteHashAsync(memoryStream, EmptyFileHash64);
                        continue;
                    }

                    var config = this.configuration.DisplayUnits[i];
                    await this.WriteHashAsync(memoryStream, config.ContentHash);
                }

                memoryStream.Position = 0;
                await this.WriteSectionAsync(memoryStream);
                await this.WriteChecksumAsync(memoryStream);
            }

            private async Task WriteSectionAsync(Stream content)
            {
                await content.CopyToAsync(this.output);
                content.Position = 0;
            }

            private async Task WriteOtherFilesSectionAsync()
            {
                await this.WriteSectionHeaderAsync(SectionType.OtherFiles, SectionVersion.Version00, 8);
                var memoryStream = new MemoryStream();
                await this.WriteHashAsync(memoryStream, this.configuration.FirmwareHash);
                memoryStream.Position = 0;
                await this.WriteSectionAsync(memoryStream);
                await this.WriteChecksumAsync(memoryStream);
            }

            private async Task WriteSchedulerTableSectionAsync()
            {
                await this.WriteSectionHeaderAsync(SectionType.SchedulerTable, SectionVersion.Version00, 84);
                var memoryStream = new MemoryStream();
                for (var i = 0; i < 12; i++)
                {
                    await this.WriteSlotAsync(memoryStream, SchedulerSlot.Slot0);
                }

                for (var i = 0; i < 12; i++)
                {
                    await this.WriteSlotAsync(memoryStream, SchedulerSlot.Slot1);
                }

                for (var i = 0; i < 12; i++)
                {
                    await this.WriteSlotAsync(memoryStream, SchedulerSlot.Slot2);
                }

                for (var i = 0; i < 12; i++)
                {
                    await this.WriteSlotAsync(memoryStream, SchedulerSlot.Slot3);
                }

                for (var i = 0; i < 12; i++)
                {
                    await this.WriteSlotAsync(memoryStream, SchedulerSlot.Slot4);
                }

                for (var i = 0; i < 12; i++)
                {
                    await this.WriteSlotAsync(memoryStream, SchedulerSlot.Slot0);
                }

                for (var i = 0; i < 12; i++)
                {
                    await this.WriteSlotAsync(memoryStream, SchedulerSlot.Slot1);
                }

                memoryStream.Position = 0;
                await this.WriteSectionAsync(memoryStream);
                await this.WriteChecksumAsync(memoryStream);
            }

            private async Task WriteSlotAsync(Stream stream, SchedulerSlot slot)
            {
                var slotString = ((byte)slot).ToString();
                var hex = HexStringToByteArray(slotString + slotString);
                await stream.WriteAsync(hex, 0, hex.Length);
            }

            private async Task WriteChecksumAsync(Stream inputStream)
            {
                var hash = ContentResourceHash.Create(inputStream, HashAlgorithmTypes.xxHash32);
                var bytes = HexStringToByteArray(hash);
                await this.output.WriteAsync(bytes, 0, bytes.Length);
            }

            private async Task WriteHashAsync(Stream stream, string hash)
            {
                var bytes = HexStringToByteArray(hash);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }

            private async Task WriteSectionHeaderAsync(SectionType type, SectionVersion version, ushort payloadSize)
            {
                this.output.WriteByte((byte)type);
                this.output.WriteByte((byte)version);
                var bytes = BitConverter.GetBytes(payloadSize);
                var correct = bytes.Reverse();
                await this.output.WriteAsync(correct.ToArray(), 0, bytes.Length);
            }
        }
    }
}
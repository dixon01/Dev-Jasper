// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterLayout.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the configuration for a master layout.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the configuration for a master layout.
    /// </summary>
    [XmlRoot("MasterLayout")]
    public class MasterLayout : ICloneable
    {
        private List<string> columnWidths;
        private List<int> gapWidths;
        private List<int> gapHeights;
        private List<string> rowHeights;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterLayout"/> class.
        /// </summary>
        public MasterLayout()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterLayout"/> class.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        public MasterLayout(MasterLayout layout)
        {
            this.Columns = layout.Columns;
            this.Rows = layout.Rows;
            this.Name = layout.Name;
            this.HorizontalGaps = layout.HorizontalGaps;
            this.VerticalGaps = layout.VerticalGaps;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the columns of the physical screen as ; separated string.
        /// </summary>
        [XmlAttribute("Columns")]
        public string Columns { get; set; }

        /// <summary>
        /// Gets or sets the horizontal gaps between the columns as ; separated string.
        /// </summary>
        [XmlAttribute("HorizontalGaps")]
        public string HorizontalGaps { get; set; }

        /// <summary>
        /// Gets or sets the rows of the physical screen as ; separated string.
        /// </summary>
        [XmlAttribute("Rows")]
        public string Rows { get; set; }

        /// <summary>
        /// Gets or sets the vertical gaps between the rows as ; separated string.
        /// </summary>
        [XmlAttribute("VerticalGaps")]
        public string VerticalGaps { get; set; }

        /// <summary>
        /// Gets the column widths.
        /// </summary>
        [XmlIgnore]
        public List<string> ColumnWidths
        {
            get
            {
                return this.columnWidths ?? (this.columnWidths = this.Columns.Split(new[] { ';' }).ToList());
            }
        }

        /// <summary>
        /// Gets the row heights.
        /// </summary>
        [XmlIgnore]
        public List<string> RowHeights
        {
            get
            {
                return this.rowHeights ?? (this.rowHeights = this.Rows.Split(new[] { ';' }).ToList());
            }
        }

        /// <summary>
        /// Gets the gap widths.
        /// </summary>
        [XmlIgnore]
        public List<int> GapWidths
        {
            get
            {
                if (this.gapWidths == null)
                {
                    this.gapWidths = new List<int>();
                    var widths = this.HorizontalGaps.Split(';').ToList();
                    foreach (var width in widths)
                    {
                        this.gapWidths.Add(int.Parse(width));
                    }
                }

                return this.gapWidths;
            }
        }

        /// <summary>
        /// Gets the gap heights.
        /// </summary>
        [XmlIgnore]
        public List<int> GapHeights
        {
            get
            {
                if (this.gapHeights == null)
                {
                    this.gapHeights = new List<int>();
                    var heights = this.VerticalGaps.Split(';').ToList();
                    foreach (var height in heights)
                    {
                        this.gapHeights.Add(int.Parse(height));
                    }
                }

                return this.gapHeights;
            }
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            return new MasterLayout(this);
        }
    }
}

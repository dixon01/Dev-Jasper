// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NextStopListItem.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NextStopListItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    /// <summary>
    /// Container of next stop message
    /// </summary>
    public class NextStopListItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NextStopListItem"/> class.
        /// Default constructor.
        /// </summary>
        public NextStopListItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NextStopListItem"/> class.
        /// Constructor of Departure
        /// </summary>
        /// <param name="city">
        /// The city.
        /// </param>
        /// <param name="stopName">
        /// The stop Name.
        /// </param>
        /// <param name="time">
        /// Time to departure (count down or absolute time)
        /// </param>
        public NextStopListItem(string city, string stopName, string time)
        {
            this.City = city;
            this.StopName = stopName;
            this.Time = time;
        }

        /// <summary>
        /// Gets or sets City.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the TAG "StopName".
        /// </summary>
        public string StopName { get; set; }

        /// <summary>
        /// Gets or sets Time.
        /// </summary>
        public string Time { get; set; }
    }
}

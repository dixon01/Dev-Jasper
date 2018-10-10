namespace Luminator.PresentationPlayLogging.DataStore.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Luminator.PresentationPlayLogging.DataStore.Interfaces;

    public class Item : IItem
    {
        public Item()
        {
            this.Created = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        /// <summary>Gets or sets the created.</summary>
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
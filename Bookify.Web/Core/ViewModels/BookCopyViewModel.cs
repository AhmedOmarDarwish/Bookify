﻿namespace Bookify.Web.Core.ViewModels
{
    public class BookCopyViewModel
    {
        public int Id { get; set; }

        public bool IsAvailableForRental { get; set; }

        public int EditionNumber { get; set; }

        public int SerialNumber { get; set; }

        public string? BookTitle { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}

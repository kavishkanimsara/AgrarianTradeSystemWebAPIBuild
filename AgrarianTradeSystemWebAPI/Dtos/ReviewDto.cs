﻿namespace AgrarianTradeSystemWebAPI.Dtos
{
	public class ReviewDto
	{
		public int ReviewId { get; set; }
		public int OrderID { get; set; }
		public string? ProductTitle { get; set; } = string.Empty;
        public string? ProductType { get; set; } = string.Empty;
        public DateTime OrderedDate { get; set; }
        public string? ProductDescription { get; set; }
		public string? ProductImageUrl { get; set; } = string.Empty;
		public string? Comment { get; set; } = string.Empty;
		public string? ReviewImageUrl { get; set; } = string.Empty;
		public DateTime ReviewDate { get; set; } = DateTime.Now;
		public int SellerRating { get; set; }
		public int DeliverRating { get; set; }
		public int ProductRating { get; set; }
	}
}

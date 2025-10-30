using System.ComponentModel.DataAnnotations;

namespace Admin.API.NewFolder
{
  
        public class Product
        {
            [Key]
            public Guid ProductId { get; set; }

            [Required]
            public string Name { get; set; } = "";

            [Required]
            public string Description { get; set; } = "";

            [Required]
            public decimal Price { get; set; }

            [Required]
            public string Category { get; set; } = "";

            public bool InStock { get; set; }

            public string? ImageUrl { get; set; }
        public Guid Usersid { get;  set; }
    }




    public class ProductUploadDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool InStock { get; set; }
        public IFormFile UploadedFile { get; set; }
    }
    public class ProductDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool InStock { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class ProductUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool InStock { get; internal set; }
    }
}



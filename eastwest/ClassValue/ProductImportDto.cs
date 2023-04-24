namespace eastwest.ClassValue
{
    public class ProductImportDto
    {
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string UPC { get; set; }
        public List<string> Locations { get; set; }
        public int Quantity { get; set; }
        public string Images { get; set; }
    }
}
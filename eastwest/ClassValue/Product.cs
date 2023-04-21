namespace eastwest.ClassValue
{
    public class Product
    {
        public string? SKU_product { get; set; }
        public string? Product_Name { get; set; }
        public string? UPC { get; set; }
        public List<Image> image { get; set; }
        public List<Image> arrImageAdd { get; set; }
        public List<Image> arrImageDel { get; set; }
    }
}
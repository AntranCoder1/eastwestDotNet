namespace eastwest.Utils
{
    public class FileExtension
    {
        public FileExtension()
        {

        }

        public bool IsFileExtension(string fileName)
        {
            string[] allowedExtensions = { ".xls", ".xlsx" };
            return allowedExtensions.Any(ext => fileName.EndsWith(ext));
        }
    }
}

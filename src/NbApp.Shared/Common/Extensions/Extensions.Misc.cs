// ReSharper disable once CheckNamespace
namespace Common
{
    public static partial class Extensions
    {
        public static string FormatAsFileSize(this long fileSize)
        {
            if (fileSize < 0)
            {
                return "0";
            }
            else if (fileSize >= 1024 * 1024 * 1024) //文件大小大于或等于1024MB
            {
                return string.Format("{0:0.00} GB", (double)fileSize / (1024 * 1024 * 1024));
            }
            else if (fileSize >= 1024 * 1024) //文件大小大于或等于1024KB
            {
                return string.Format("{0:0.00} MB", (double)fileSize / (1024 * 1024));
            }
            else if (fileSize >= 1024) //文件大小大于等于1024bytes
            {
                return string.Format("{0:0.00} KB", (double)fileSize / 1024);
            }
            else
            {
                return string.Format("{0} bytes", fileSize);
            }
        }
        public static string FormatAsFileSize(this long? fileSize)
        {
            if (fileSize == null)
            {
                return 0L.FormatAsFileSize();
            }
            return fileSize.Value.FormatAsFileSize();
        }

        public static string FormatAsFileSize(this int fileSize)
        {
            return ((long)fileSize).FormatAsFileSize();
        }
        public static string FormatAsFileSize(this int? fileSize)
        {
            return ((long?)fileSize).FormatAsFileSize();
        }
    }
}

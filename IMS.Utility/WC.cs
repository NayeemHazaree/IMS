using System.Text;

namespace IMS.Utility
{
    public static class WC
    {
        public static string? p_image_path = @"\images\product\";
        public static string? SessionCart = "ShoppingCartSession";
        public static string? OrderCart = "OrderListSession";
        public static string? AdminMail = "tmy0044@gmail.com";
        public static string? Pending = "Pending";
        public static string? Submitted = "Submitted";
        public static string? Cancel = "Cancel";

        public static string GenerateRandomString()
        {
            int length = 3;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            //System.Console.WriteLine(str_build.ToString());
            return str_build.ToString();
        }
    }
}

using WebApp.Core;
using WebApp.Models;

namespace WebApp.Helpers
{
    public static class Helper
    {
        public static void FromRSA(Customer customer,User keys)
        {
            Rsa rsa = new Rsa();

            customer.FullName = rsa.decode(customer.FullName, keys.Key_N, keys.Key_D);
            customer.Address = rsa.decode(customer.Address, keys.Key_N, keys.Key_D);
            customer.City = rsa.decode(customer.City, keys.Key_N, keys.Key_D);
            customer.ZipCode = rsa.decode(customer.ZipCode, keys.Key_N, keys.Key_D);
            customer.Country = rsa.decode(customer.Country, keys.Key_N, keys.Key_D);
            //customer.FullName = rsa.decode(customer.FullName, keys.Key_N, keys.Key_D);

        }
    }
}
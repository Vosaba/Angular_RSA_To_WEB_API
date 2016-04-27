using WebApp.Core;
using WebApp.Models;

namespace WebApp.Helpers
{
    public static class Helper
    {
        public static void FromRSA(Customer customer,User keys)
        {
            Rsa rsa = new Rsa();

            customer.FullName = rsa.Decode(customer.FullName, keys.Key_N, keys.Key_D);
            customer.Address = rsa.Decode(customer.Address, keys.Key_N, keys.Key_D);
            customer.City = rsa.Decode(customer.City, keys.Key_N, keys.Key_D);
            customer.ZipCode = rsa.Decode(customer.ZipCode, keys.Key_N, keys.Key_D);
            customer.Country = rsa.Decode(customer.Country, keys.Key_N, keys.Key_D);
            //customer.FullName = rsa.decode(customer.FullName, keys.Key_N, keys.Key_D);

        }

        public static void ToRSA(Customer customer, string nkey, string ekey)
        {
            Rsa rsa = new Rsa();

            customer.FullName = rsa.Encode(customer.FullName, nkey, ekey);
            customer.Address = rsa.Encode(customer.Address, nkey, ekey);
            customer.City = rsa.Encode(customer.City, nkey, ekey);
            customer.ZipCode = rsa.Encode(customer.ZipCode, nkey, ekey);
            customer.Country = rsa.Encode(customer.Country, nkey, ekey);
            //customer.FullName = rsa.decode(customer.FullName, keys.Key_N, keys.Key_D);

        }
    }
}
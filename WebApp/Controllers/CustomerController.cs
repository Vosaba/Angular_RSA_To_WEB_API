using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApp.Core;
using WebApp.DataAccessLayer;
using WebApp.Helpers;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class CustomerController : ApiController
    {
        DataContext db = new DataContext();
        [Route("api/Customer/search")]
        [HttpGet]
        public IEnumerable<Customer> SearchCustomers(string searchText,string nkey,string ekey)
        {

//var rsa = new Rsa();
  //          searchText = rsa.Encode(searchText, nkey, ekey);
         


            searchText = searchText ?? "";
           var customers = db.Customers
                .Where(x => x.FullName.Contains(searchText) ||
                        x.Country.Contains(searchText) ||
                        x.City.Contains(searchText)
                )
                .Take(20).ToList();

         // customers.ForEach(e => Helper.ToRSA(e,nkey,ekey));

            return customers;
        }

        [Route("api/Customer/new")]
        [HttpGet]
        public Customer NewCustomer()
        {
            return new Customer();
        }


        [Route("api/Customer/getKey")]
        [HttpGet]
        public IHttpActionResult GetKey()
        {
            var rsa = new Rsa();
            rsa.InitKeyData();

           var user =   db.UsersKey.FirstOrDefault(x => x.UserId == 123);
            if (user == null)
            {
                db.UsersKey.Add(new User()
                {
                    Key_D = rsa.GetDKey().ToString(),
                    Key_N = rsa.GetNKey().ToString(),
                    UserId = 123
                });
            }
            else
            {
                user.Key_D = rsa.GetDKey().ToString();
                user.Key_N = rsa.GetNKey().ToString();
            }
            

            db.SaveChanges();
            return Json(new {Ekey= rsa.GetEKey(),Nkey=rsa.GetNKey()});
        }

        [Route("api/Customer/detail/{id}")]
        [HttpGet]
        public Customer GetDetail(int Id)
        {
            return db.Customers.FirstOrDefault(x=>x.CustomerId == Id);
        }

        [Route("api/Customer/Save")]
        [HttpPost]
        public Customer SaveCustomer(Customer customer)
        {
            var keys = db.UsersKey.First(e => e.UserId == 123);

            Helper.FromRSA(customer,keys);

            if (customer.CustomerId > 0)
            {
                var dbCustomer = db.Customers.FirstOrDefault(x => x.CustomerId == customer.CustomerId);
                dbCustomer.FullName = customer.FullName;
                dbCustomer.Address = customer.Address;
                dbCustomer.City = customer.City;
                dbCustomer.Country = customer.Country;
                dbCustomer.ZipCode = customer.ZipCode;
                db.SaveChanges();
                return dbCustomer;
            }
            else
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return customer;
            }
        }

        [Route("api/Customer/delete/{id}")]
        [HttpPost]
        public void DeleteCustomer(int Id)
        {
            var customer = db.Customers.FirstOrDefault(x => x.CustomerId == Id);
            db.Customers.Remove(customer);
            db.SaveChanges();
        }
    }
}

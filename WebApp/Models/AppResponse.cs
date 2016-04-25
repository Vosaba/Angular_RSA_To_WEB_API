using System;

namespace WebApp.Models
{
    public class AppResponse
    {
        public bool Status { get; set; }
        public String Message { get; set; }
        public Object Data { get; set; }
    }
}
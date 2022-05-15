using System;

namespace CoffeeAppWebApi.DataEf
{
    public class UserVerificationCode
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int Code { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}

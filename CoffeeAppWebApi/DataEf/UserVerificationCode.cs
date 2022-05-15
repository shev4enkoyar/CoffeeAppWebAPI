using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeAppWebApi.DataEf
{
    public class UserVerificationCode
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int Code { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ExpirationDate { get; set; }
    }
}

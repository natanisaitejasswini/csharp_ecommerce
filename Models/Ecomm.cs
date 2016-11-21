using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecomm.Models
{
    public abstract class BaseEntity {}

    public class User : BaseEntity
        {
            public int id;
            [Required]
            [MinLength(2)]
            [RegularExpression(@"^[a-zA-Z]+$")]
            public string first_name { get; set; }
            [Required]
            [MinLength(1)]
            [RegularExpression(@"^[a-zA-Z]+$")]
            public string last_name { get; set; }
            [Required]
            [EmailAddress]
            public string email{ get; set; }
            [Required]
            [MinLength(3)]
            public string password { get; set; }
            [Required]
            [Compare("password")]
            public string confirm_password {get; set;}
            public DateTime created_at;
            public DateTime updated_at;
        }
        public class Product : BaseEntity
            {
                public int id;
                [Required]
                [MinLength(2)]
                public string product_name { get; set; }
                [Required]
                [MinLength(2)]
                public string description { get; set; }
                [Required]
                [MinLength(2)]
                public string image{get; set;}
                [Required]
                public int quantity{get; set;}
                [Required]
                public double price{get; set;}
                public DateTime created_at;
                public DateTime updated_at;
                public string first_name {get; set;}
                public int user_id {get; set;}
            }
            public class Order : BaseEntity
            {
                public int id;
                [Required]
                public int quantity { get; set; }
                public DateTime created_at;
                public DateTime updated_at;
                public double price {get; set;}
                public string first_name {get; set;}
                public string product_name {get; set;}
                public int user_id {get; set;}
                public int product_id {get; set;}
            }
}

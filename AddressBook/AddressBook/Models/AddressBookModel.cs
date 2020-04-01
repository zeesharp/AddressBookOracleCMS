using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddressBook.Models
{
    public class AddressBookModel
    {

        public int? ID { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile Number")]
        [Display(Name = "Mobile Number")]
        [Phone]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Provided phone number not valid")]

        public string MobileNumber { get; set; }
    }
}
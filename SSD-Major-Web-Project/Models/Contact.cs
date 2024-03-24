using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.Models;

public partial class Contact
{
    public int PkContactId { get; set; }

    [Required(ErrorMessage = "Frist Name cannot be empty.")]
    public string FirstName { get; set; } = null!;
    [Required(ErrorMessage = "Last Name cannot be empty.")]
    public string LastName { get; set; } = null!;
    [Required(ErrorMessage = "Address cannot be empty.")]
    public string Address { get; set; } = null!;

    public string? Address2 { get; set; }
    [Required(ErrorMessage = "City cannot be empty.")]
    public string City { get; set; } = null!;
    [Required(ErrorMessage = "Province cannot be empty.")]
    public string Province { get; set; } = null!;
    [Required(ErrorMessage = "Country cannot be empty.")]
    public string Country { get; set; } = null!;
    [Required(ErrorMessage = "Postal code cannot be empty.")]
    public string PostalCode { get; set; } = null!;
    [Required(ErrorMessage = "Phone Number cannot be empty.")]
    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Data.Entities;


// Adding some data annotations
[Table ("Patients")]
public  class Patient
{
    public int Id {get;set;}

    // Data annotations can be stacked or inline in the same brackets.
    // they apply to the property directly below them. No fall through.
    [Required, MaxLength(100)]
    public string FirstName {get;set;} = default!;

    [Required, MaxLength(100)]
    public string LastName {get;set;} = default!;
    [Required]
    public DateTime DateOfBirth {get;set;} = default!;
    [Required,MaxLength(100)]
    public string Insurance {get;set;} = default!;
    [MaxLength(100)]
    public string Email {get;set;}

    public string ContactNumber {get;set;}

    public string EmergencyContactName {get;set;}

    public string EmergencyContactNumber {get;set;}
    [Required,MaxLength(5)]
    public string BloodType {get;set;} = default!;
    public string Address {get;set;} = default!;

    public string Allergies {get;set;} = default!;

    // One customer can have many orders
    public  List<Order> Orders {get;set;} = new(); // Good idea to initialize to empty list

}
using System.ComponentModel.DataAnnotations;

namespace sistem_e_daftar_gemaputera.Models;

public class User
{
    public string FullName {get; set;}
    
    [Key]
    public string Email {get; set;}
    public string Password {get;set;}
    public string Contact {get;set;}
    public string Designation { get; set; }
    public string Agency  { get; set; }
}

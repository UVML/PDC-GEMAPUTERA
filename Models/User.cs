using System.ComponentModel.DataAnnotations;

namespace sistem_e_daftar_gemaputera.Models;

public class User
{
    [Key]
    public string Agency  { get; set; }
    public string FullName {get; set;}     
    public string Email {get; set;}
    public string Password {get;set;}
    public string Contact {get;set;}
    public string Designation { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace sistem_e_daftar_gemaputera.Models;

public class Pengguna
{
    [Key]
    public string Agensi  { get; set; }
    public string NamaPenuh {get; set;}     
    public string Emel {get; set;}
    public string KataLaluan {get;set;}
    public string Telefon {get;set;}
    public string Jawatan { get; set; }

    public string? FileYuran { get; set; }   
    public string? FileResit { get; set; }
    public DateTime? TarikhHantar { get; set; }
    public DateTime? TarikhResit { get; set; }
}

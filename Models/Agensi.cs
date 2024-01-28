using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistem_e_daftar_gemaputera.Models;

public class Agensi
{
    [Key]
    public string Nama  { get; set; }

    public DateTime TarikhTutup { get; set; }

    [NotMapped]
    public string TarikhTutupForInput { 
        get {
            return TarikhTutup.ToString("yyyy-MM-dd");
        }
    }
}

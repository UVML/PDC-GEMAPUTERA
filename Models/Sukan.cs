using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace sistem_e_daftar_gemaputera.Models;

public class Sukan
{
    [Key]
    public string Nama  { get; set; }
    public decimal Fee {get; set;}     
    public string Kategori {get; set;}
    public string Konfigurasi {get;set;}  //Pengurus=1, Jurulatih=1, Fisio=0, Pemain=10
    
    [NotMapped]
    public class Config{
        public string JenisAhli {get; set;}
        public int Size {get; set;}
    }

    [NotMapped]
    public List<Config> KonfigurasiAhli {
        get 
        {
            var _config = Konfigurasi.Split(',').Select(x=>x.Split('=')).Select(x=>new Config{JenisAhli=x[0].Trim(), Size=Convert.ToInt32(x[1])}).ToList();
            return _config; 
        }
    
    }
}



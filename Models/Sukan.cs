using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;


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
    public Bilangan BilanganJenisAhli {
        get
        {
            return JsonConvert.DeserializeObject<Bilangan>(Konfigurasi);            
        }
    }

    [NotMapped]
    public string[] KategoryAsList{
        get
        {
            return Kategori.Split(',').Select(x=>x.Trim()).ToArray();
        }
    }

    [NotMapped]
    public class Bilangan {
        public int Pengurus {get; set;}
        public int Jurulatih {get; set;}
        public int Fisio {get; set;}
        public int Pemain {get; set;}
    }
}



using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistem_e_daftar_gemaputera.Models;

public class Ahli
{
    [Key]
    public int Id { get; set; }
    public string Agensi { get; set; }
    public string JenisSukan { get; set; }
    public string JenisAhli { get; set; }
    public string? NamaPenuh { get; set; }
    public string? NoKP { get; set; }
    public string? NoPekerja { get; set; }
    public string? NoKWSP { get; set; }
    public string? TarafJawatan { get; set; }
    public string? KumpulanJawatan { get; set; }
    public string? KategoriPermainan { get; set; }
    public string? FileGambar { get; set; }
    public string? FileKP { get; set; }
    public string? FileKWSP { get; set; }
    public string? FileMajikan { get; set; }
    public string? FileSuratLantikan { get; set; }
    public string? Telefon { get; set; }
    public string? GredJawatan { get; set; }

    public string? StatusPemain {get; set;}

    

    [NotMapped]
    public bool IsCompleted
    {
        get
        {

            if (string.IsNullOrEmpty(NamaPenuh) == false &&
                string.IsNullOrEmpty(NoKP) == false &&
                (JenisAhli=="Jurulatih" ? true : string.IsNullOrEmpty(NoPekerja) == false) &&
                (JenisAhli=="Jurulatih" ? true : string.IsNullOrEmpty(NoKWSP) == false) &&
                (JenisAhli=="Jurulatih" ? true :string.IsNullOrEmpty(TarafJawatan) == false) &&
                (JenisAhli=="Jurulatih" ? true : string.IsNullOrEmpty(KumpulanJawatan) == false) &&
                (JenisAhli=="Pemain" ? string.IsNullOrEmpty(KategoriPermainan) == false : true) &&
                string.IsNullOrEmpty(FileGambar) == false &&
                string.IsNullOrEmpty(FileKP) == false &&
                (JenisAhli=="Jurulatih" ? true : string.IsNullOrEmpty(FileKWSP) == false) &&
                (JenisAhli=="Jurulatih" ? true :string.IsNullOrEmpty(FileMajikan) == false) &&                
                string.IsNullOrEmpty(Telefon) == false &&
                (JenisAhli=="Jurulatih" ? true :string.IsNullOrEmpty(GredJawatan) == false) &&
                (JenisAhli=="Jurulatih" || JenisAhli=="Pengurus" ? true :string.IsNullOrEmpty(StatusPemain) == false)

            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


}


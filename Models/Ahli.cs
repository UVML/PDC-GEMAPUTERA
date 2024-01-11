using System.ComponentModel.DataAnnotations;

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
}



using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CosmosDb.Models;

public enum HenvendelsesKategori
{
    TekniskSpørgsmål,       
    GørDetSelvReservedele,  
    ForslagTilÆndringer,    
    NærmesteForhandler,     
    KatalogAnmodning,       //Send mig det nyeste katalog
    Andet
}

public class SupportHenvendelse
{
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Display(Name = "Navn")]
    [Required(ErrorMessage = "Angiv dit navn.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Navn skal være mellem 2 og 100 tegn.")]
    public string Navn { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "Angiv din e-mail.")]
    [EmailAddress(ErrorMessage = "E-mailadressen er ikke gyldig.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Telefon")]
    [Phone(ErrorMessage = "Telefonnummeret er ikke gyldigt.")]
    [StringLength(30)]
    public string? Telefon { get; set; }

    [Display(Name = "Kategori")]
    [Required(ErrorMessage = "Vælg en kategori.")]
    public HenvendelsesKategori Kategori { get; set; } = HenvendelsesKategori.Andet;

    [Display(Name = "Beskrivelse")]
    [Required(ErrorMessage = "Skriv en kort beskrivelse.")]
    [StringLength(4000, MinimumLength = 5, ErrorMessage = "Beskrivelsen skal være mindst 5 tegn.")]
    [DataType(DataType.MultilineText)]
    public string Beskrivelse { get; set; } = string.Empty;

    [Display(Name = "Oprettet (UTC)")]
    [DataType(DataType.DateTime)]
    public DateTime OprettetTidspunktUtc { get; set; } = DateTime.UtcNow;
}

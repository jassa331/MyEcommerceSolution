using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("seller", Schema = "dbo")]
public class SELLERFOAM
{
    [Key]
    [JsonIgnore]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid SellerId { get; set; } = Guid.NewGuid();


    public string Name { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Category { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public string PAN { get; set; }
    public string Aadhaar { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedDate { get; set; }
}

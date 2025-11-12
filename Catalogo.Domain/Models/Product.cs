using System.Text.Json.Serialization;

namespace Catalogo.Domain.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double Price { get; set; }
    public int UserId { get; set; }
    [JsonIgnore]
    public User? user { get; set; }
}
using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class Category
{
    public byte CategoryId { get; set; } // tinyint
    public string Name { get; set; } = string.Empty;
    public DateTime LastUpdate { get; set; } = DateTime.Now;
}
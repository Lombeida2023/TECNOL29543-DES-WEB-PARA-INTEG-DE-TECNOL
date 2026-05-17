using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SakilaApp.Models;

[Table("store")]
public class Store
{
    [Key]
    [Column("store_id")]
    public int StoreId { get; set; }

    [Column("manager_staff_id")]
    public byte ManagerStaffId { get; set; }

    [Column("address_id")]
    public int AddressId { get; set; }

    [Column("last_update")]
    public DateTime LastUpdate { get; set; }
}
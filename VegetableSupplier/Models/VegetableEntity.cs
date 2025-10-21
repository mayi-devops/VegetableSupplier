using SQLite;
using System.Text.Json;

namespace VegetableSupplier.Models;

[Table("Vegetables")]
public class VegetableEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string NameEn { get; set; }
    public string NameHi { get; set; }
    public string NameTe { get; set; }
    public string UnitPricesJson { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Dictionary<string, decimal> GetUnitPrices()
    {
        if (string.IsNullOrEmpty(UnitPricesJson))
            return new Dictionary<string, decimal>();

        return JsonSerializer.Deserialize<Dictionary<string, decimal>>(UnitPricesJson);
    }

    public void SetUnitPrices(Dictionary<string, decimal> unitPrices)
    {
        UnitPricesJson = JsonSerializer.Serialize(unitPrices);
    }

    public Vegetable ToVegetable()
    {
        return new Vegetable
        {
            Id = Id,
            NameEn = NameEn,
            NameHi = NameHi,
            NameTe = NameTe,
            UnitPrices = GetUnitPrices(),
            UpdatedAt = UpdatedAt
        };
    }
}
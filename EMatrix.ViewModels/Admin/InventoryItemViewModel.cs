using System.ComponentModel;

namespace EMatrix.ViewModels.Admin;

public class InventoryItemViewModel
{
    [DisplayName("Код")]
    public string Id { get; set; } = null!;

    [DisplayName("Име")]
    public string NameAlias  { get; set; } = null!;

    [DisplayName("Цена")]
    public float Price { get; set; }

    [DisplayName("Количество")]
    public float Quantity { get; set; }
}
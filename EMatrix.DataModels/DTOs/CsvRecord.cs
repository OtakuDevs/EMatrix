namespace EMatrix.DataModels.DTOs;

public class CsvRecord
{
    public string ОсновнаГрупа { get; set; }
    public string Подгрупа { get; set; }
    public int Код { get; set; }
    public string Име { get; set; }
    public string Описание { get; set; }
    public string Мярка { get; set; }
    public float Цена { get; set; }
    public float Количество { get; set; }
}
using EMatrix.UtilityServices.Interfaces;
using EMatrix.ViewModels.Services;

namespace EMatrix.UtilityServices;

public class ToolsService : IToolsService
{
    public List<ResistorBandModel> GetResistorColorCodeModel()
    {
        var model = new List<ResistorBandModel>();
        //Black
        model.Add(new ResistorBandModel
        {
            Color = "Черен",
            Class = "black",
            SecondDigit = 0,
            ThirdDigit = 0,
            Multiplier = 1,
        });
        //Brown
        model.Add(new ResistorBandModel()
        {
            Color = "Кафяв",
            Class = "brown",
            FirstDigit = 1,
            SecondDigit = 1,
            ThirdDigit = 1,
            Multiplier = 10,
            Tolerance = 1,
            TempCoefficient = 100
        });
        //Red
        model.Add(new ResistorBandModel()
        {
            Color = "Червен",
            Class = "red",
            FirstDigit = 2,
            SecondDigit = 2,
            ThirdDigit = 2,
            Multiplier = 100,
            Tolerance = 2,
            TempCoefficient = 50
        });
        //Orange
        model.Add(new ResistorBandModel()
        {
            Color = "Оранжев",
            Class = "orange",
            FirstDigit = 3,
            SecondDigit = 3,
            ThirdDigit = 3,
            Multiplier = 1000,
            Tolerance = 0.05,
            TempCoefficient = 15
        });
        //Yellow
        model.Add(new ResistorBandModel()
        {
            Color = "Жълто",
            Class = "yellow",
            FirstDigit = 4,
            SecondDigit = 4,
            ThirdDigit = 4,
            Multiplier = 10000,
            Tolerance = 0.02,
            TempCoefficient = 25
        });
        //Green
        model.Add(new ResistorBandModel()
        {
            Color = "Зелен",
            Class = "green",
            FirstDigit = 5,
            SecondDigit = 5,
            ThirdDigit = 5,
            Multiplier = 100000,
            Tolerance = 0.5,
        });
        //Blue
        model.Add(new ResistorBandModel()
        {
            Color = "Син",
            Class = "blue",
            FirstDigit = 6,
            SecondDigit = 6,
            ThirdDigit = 6,
            Multiplier = 1000000,
            Tolerance = 0.25,
            TempCoefficient = 10
        });
        //Purple
        model.Add(new ResistorBandModel()
        {
            Color = "Лилав",
            Class = "purple",
            FirstDigit = 7,
            SecondDigit = 7,
            ThirdDigit = 7,
            Multiplier = 10000000,
            Tolerance = 0.1,
            TempCoefficient = 5
        });
        //Gray
        model.Add(new ResistorBandModel()
        {
            Color = "Сив",
            Class = "gray",
            FirstDigit = 8,
            SecondDigit = 8,
            ThirdDigit = 8,
            Multiplier = 100000000,
            Tolerance = 0.01
        });
        //White
        model.Add(new ResistorBandModel()
        {
            Color = "Бял",
            Class = "white",
            FirstDigit = 9,
            SecondDigit = 9,
            ThirdDigit = 9,
            Multiplier = 1000000000,
            TempCoefficient = 1
        });
        //Gold
        model.Add(new ResistorBandModel()
        {
            Color = "Златен",
            Class = "gold",
            Multiplier = 0.1,
            Tolerance = 5
        });
        //Silver
        model.Add(new ResistorBandModel()
        {
            Color = "Сребърен",
            Class = "silver",
            Multiplier = 0.01,
            Tolerance = 10
        });
        //Pink
        model.Add(new ResistorBandModel()
        {
            Color = "Розов",
            Class = "pink",
            Multiplier = 0.001,
        });
        return model;
    }
}
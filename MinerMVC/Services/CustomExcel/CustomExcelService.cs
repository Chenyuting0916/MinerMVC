using MinerMVC.Data;
using MinerMVC.Models.CustomExcelDb;

namespace MinerMVC.Services;

public class CustomExcelService : ICustomExcelService
{
    private readonly CustomExcelDbContext _customExcelDbContext;

    public CustomExcelService(CustomExcelDbContext customExcelDbContext)
    {
        _customExcelDbContext = customExcelDbContext;
    }

    public void Insert(CustomExcel customExcel)
    {
        _customExcelDbContext.CustomExcels.Add(customExcel);
        _customExcelDbContext.SaveChanges();
    }

    public List<CustomExcel> GetAll()
    {
        return _customExcelDbContext.CustomExcels.ToList();
    }

    public void Delete(int id)
    {
        var customExcel = _customExcelDbContext.CustomExcels.First(x => x.Id == id);
        _customExcelDbContext.CustomExcels.Remove(customExcel);
        _customExcelDbContext.SaveChanges();
    }

    public CustomExcel Get(int id)
    {
        return _customExcelDbContext.CustomExcels.First(x => x.Id == id);
    }

    public void Edit(CustomExcel customExcel)
    {
        var dbCustomExcel = _customExcelDbContext.CustomExcels.First(x=>x.Id == customExcel.Id);
        dbCustomExcel.Description = customExcel.Description;
        dbCustomExcel.Name = customExcel.Name;
        dbCustomExcel.ImageName = customExcel.ImageName;
        dbCustomExcel.Verified = customExcel.Verified;
        _customExcelDbContext.SaveChanges();
    }
}
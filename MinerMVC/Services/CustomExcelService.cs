using MinerMVC.Data;
using MinerMVC.Models.CustomExcelDb;
using MinerMVC.ViewModel;

namespace MinerMVC.Services;

public class CustomExcelService : ICustomExcelService
{
    private readonly CustomExcelDbContext _customExcelDbContext;

    public CustomExcelService(CustomExcelDbContext customExcelDbContext)
    {
        _customExcelDbContext = customExcelDbContext;
    }

    public void Insert(CustomExcelViewModel viewModel)
    {
        var customExcel = new CustomExcel()
        {
            Description = viewModel.Description,
            Name = viewModel.Name,
            Verified = viewModel.Verified,
            ImageName = viewModel.ImageName
        };
        _customExcelDbContext.CustomExcels.Add(customExcel);
        _customExcelDbContext.SaveChanges();
    }

    public List<CustomExcelViewModel> GetAll()
    {
        return _customExcelDbContext.CustomExcels.Select(x => new CustomExcelViewModel(x)).ToList();
    }

    public void Delete(int id)
    {
        var customExcel = _customExcelDbContext.CustomExcels.First(x => x.Id == id);
        _customExcelDbContext.CustomExcels.Remove(customExcel);
        _customExcelDbContext.SaveChanges();
    }

    public CustomExcelViewModel Get(int id)
    {
        return new CustomExcelViewModel(_customExcelDbContext.CustomExcels.First(x => x.Id == id));
    }
}
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
            ImagePath = viewModel.ImagePath
        };
        _customExcelDbContext.CustomExcels.Add(customExcel);
        _customExcelDbContext.SaveChanges();
    }

    public List<CustomExcelViewModel> GetAll()
    {
        return _customExcelDbContext.CustomExcels.Select(x => new CustomExcelViewModel()
        {
            Description = x.Description,
            Id = x.Id,
            Name = x.Name,
            Verified = x.Verified,
            ImagePath = x.ImagePath
        }).ToList();
    }
}
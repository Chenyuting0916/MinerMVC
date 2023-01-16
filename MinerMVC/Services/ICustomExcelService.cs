using MinerMVC.ViewModel;

namespace MinerMVC.Services;

public interface ICustomExcelService
{
    public void Insert(CustomExcelViewModel customExcel);
    public List<CustomExcelViewModel> GetAll();
}
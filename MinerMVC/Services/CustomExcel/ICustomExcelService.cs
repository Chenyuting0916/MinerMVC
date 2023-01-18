using MinerMVC.Models.CustomExcelDb;

namespace MinerMVC.Services;

public interface ICustomExcelService
{
    public void Insert(CustomExcel customExcel);
    public List<CustomExcel> GetAll();
    public void Delete(int id);
    public CustomExcel Get(int id);
    public void Edit(CustomExcel model);
    void UpdateVerifiedStatus(int id, bool status);
}
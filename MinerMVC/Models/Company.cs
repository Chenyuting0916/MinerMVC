namespace MinerMVC.Models;

public class Company
{
    public Company(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}
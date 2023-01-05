namespace MinerMVC.Models;

public class Company
{
    public Company(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    // public List<Job> Jobs { get; set; }
}

// public class Job
// {
//     public string Title { get; set; }
//     public int Salary { get; set; }
// }
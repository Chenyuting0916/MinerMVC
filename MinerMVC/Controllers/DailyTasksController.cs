using Microsoft.AspNetCore.Mvc;
using MinerMVC.Data;
using MinerMVC.Models.NewFolder;
using System;

public class DailyTaskController : Controller
{
    private readonly DailyTaskDbContext _context;

    public DailyTaskController(DailyTaskDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var tasks = _context.DailyTasks.ToList();
        return View(tasks);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(DailyTask task)
    {
        if (ModelState.IsValid)
        {
            _context.Add(task);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(task);
    }

    public IActionResult Edit(int id)
    {
        var task = _context.DailyTasks.Find(id);
        return View(task);
    }

    [HttpPost]
    public IActionResult Edit(DailyTask task)
    {
        if (ModelState.IsValid)
        {
            _context.Update(task);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(task);
    }

    public IActionResult Delete(int id)
    {
        var task = _context.DailyTasks.Find(id);
        _context.Remove(task);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Details(int id)
    {
        var task = _context.DailyTasks.Find(id);
        return View(task);
    }
}

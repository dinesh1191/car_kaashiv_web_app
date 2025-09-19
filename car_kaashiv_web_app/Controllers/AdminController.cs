using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Models.Entities;
using car_kaashiv_web_app.Models.Enums;
using car_kaashiv_web_app.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace car_kaashiv_web_app.Controllers
{
  
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
      //Get AdminDashboard
        public IActionResult AdminDashboard()
        {
            return View();
        }      

        // GET: All Employee list
        public async Task<IActionResult> Index()
        {
            return View(await _context.tbl_emp.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableEmployee = await _context.tbl_emp
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableEmployee == null)
            {
                return NotFound();
            }

            return View(tableEmployee);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Phone,Email,Role,EmpPasswordHash,CreatedAt")] TableEmployee tableEmployee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tableEmployee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tableEmployee);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Employee Id not provided";              
              }

            var tableEmployee = await _context.tbl_emp.FindAsync(id);
            if (tableEmployee == null)
            {
                TempData["ErrorMessage"] = "Employee not found!";
                return RedirectToAction("AdminDashboard");              

            }
            return View(tableEmployee);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,Email,Role,EmpPasswordHash,CreatedAt")] TableEmployee tableEmployee)
        {
            if (id != tableEmployee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tableEmployee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableEmployeeExists(tableEmployee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tableEmployee);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableEmployee = await _context.tbl_emp
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableEmployee == null)
            {
                return NotFound();
            }

            return View(tableEmployee);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)

        {
            TempData.setAlert("Employee deleted successfully", AlertTypes.Success);
            TempData.setAlert("Employee deleted successfully", AlertTypes.Error);
            //var tableEmployee = await _context.tbl_emp.FindAsync(id);
            //if (tableEmployee != null)
            //{
            //    _context.tbl_emp.Remove(tableEmployee);
            //}

            //await _context.SaveChangesAsync();
            ////return View();
            ////TempData.setAlert(AlertTypes.Success, "Employee deleted successfully");
            //TempData.setAlert("Employee deleted successfully", AlertTypes.Error);
            return RedirectToAction(nameof(Index));
        }

        private bool TableEmployeeExists(int id)
        {
            return _context.tbl_emp.Any(e => e.Id == id);
        }
    }
}

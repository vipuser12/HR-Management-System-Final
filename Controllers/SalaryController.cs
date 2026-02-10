using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HRManagementSys.Data;
using HRManagementSys.Models;

namespace HRManagementSys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [Authorize(Policy = "view_employees")] 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Salary>>> GetAllSalaries()
        {
            return await _context.Salaries
                .Include(s => s.Employee) 
                .ToListAsync();
        }

        
        [Authorize(Policy = "create_employees")]
        [HttpPost("update-salary")]
        public async Task<IActionResult> UpdateSalary(Salary salary)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == salary.EmployeeId);
            if (!employeeExists) return NotFound("الموظف غير موجود");

           
            salary.EffectiveDate = DateTime.Now;
            _context.Salaries.Add(salary);

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديث الراتب بنجاح" });
        }

       
        [Authorize(Policy = "view_employees")]
        [HttpGet("total-payroll")]
        public async Task<IActionResult> GetTotalPayroll()
        {
            var total = await _context.Salaries.SumAsync(s => s.BaseSalary);
            return Ok(new { TotalMonthlyPayroll = total });
        }
    }
}
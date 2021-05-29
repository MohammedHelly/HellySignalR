using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Customer.Data;
using Customer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Customer.Controllers
{
    public class CustomerInfoesController : Controller
    {
        private readonly CustomerContext _context;
        private readonly IHubContext<CusHub> _signalContext;
        public IConfiguration Configuration { get; }
        public CustomerInfoesController(CustomerContext context, IHubContext<CusHub> signalContext, IConfiguration configuration)
        {
            _context = context;
            _signalContext = signalContext;
            Configuration = configuration;
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e) //this will be called when any changes occur in db table. 
        {
            //if (e.Type == SqlNotificationType.Change)
            //{
            //    _signalContext.Show();
            //}
            var dependency = sender as SqlDependency;

            if (dependency == null) return;

            if (e.Info == SqlNotificationInfo.Insert)
            {
                dependency.OnChange -= dependency_OnChange;

                _signalContext.Clients.All.SendAsync("displayCustomer");
            }
        }
        // GET: CustomerInfoes
        public async Task<IActionResult> Index()
        {
            // return View(await _context.CustomerInfos.ToListAsync());

            {
                var connStr = Configuration.GetConnectionString("DefaultConnection");

                try
                {

                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        string cmdText = @"SELECT [Id]
                                     ,[CusId]
                                    ,[CusName]
                                     ,[Status]
                                    FROM [Customer].[dbo].[CustomerInfo]";
                        using (SqlCommand cmd = new SqlCommand(cmdText, con))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.Notification = null;
                            if (con.State == ConnectionState.Closed)
                            {
                                await con.OpenAsync();
                            }

                            SqlDependency customerInfoDependency = new SqlDependency(cmd);
                            customerInfoDependency.OnChange += dependency_OnChange;
                            SqlDependency.Start(connStr);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                var listCus = reader.Cast<IDataRecord>()
                                .Select(x => new CustomerInfo
                                {
                                    Id = (int)x["Id"],
                                    CusId = (string)x["CusId"],
                                    CusName = (string)x["CusName"],
                                }).ToList();
                                return View(listCus);
                            }
                        }
                    }

                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }


               
            }
        }

        // GET: CustomerInfoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerInfo = await _context.CustomerInfos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerInfo == null)
            {
                return NotFound();
            }

            return View(customerInfo);
        }

        // GET: CustomerInfoes/Create
        public IActionResult Create()
        {
            return View();
        }

          [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CusId,CusName,Status")] CustomerInfo customerInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customerInfo);
        }

        // GET: CustomerInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerInfo = await _context.CustomerInfos.FindAsync(id);
            if (customerInfo == null)
            {
                return NotFound();
            }
            return View(customerInfo);
        }

          [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CusId,CusName,Status")] CustomerInfo customerInfo)
        {
            if (id != customerInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerInfoExists(customerInfo.Id))
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
            return View(customerInfo);
        }

        // GET: CustomerInfoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerInfo = await _context.CustomerInfos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerInfo == null)
            {
                return NotFound();
            }

            return View(customerInfo);
        }

        // POST: CustomerInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerInfo = await _context.CustomerInfos.FindAsync(id);
            _context.CustomerInfos.Remove(customerInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerInfoExists(int id)
        {
            return _context.CustomerInfos.Any(e => e.Id == id);
        }
    }
}

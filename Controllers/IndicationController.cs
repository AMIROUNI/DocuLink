using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocuLink.Data;
using DocuLink.Models;

namespace DocuLink.Controllers
{
    public class IndicationController : Controller
    {
        private readonly AppDbContext _context;

        public IndicationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Indication
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Indications.Include(i => i.Medication);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Indication/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indication = await _context.Indications
                .Include(i => i.Medication)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (indication == null)
            {
                return NotFound();
            }

            return View(indication);
        }

        // GET: Indication/Create
        public IActionResult Create()
        {
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Id");
            return View();
        }

        // POST: Indication/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ConditionName,Description,MedicationId")] Indication indication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(indication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Id", indication.MedicationId);
            return View(indication);
        }

        // GET: Indication/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indication = await _context.Indications.FindAsync(id);
            if (indication == null)
            {
                return NotFound();
            }
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Id", indication.MedicationId);
            return View(indication);
        }

        // POST: Indication/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ConditionName,Description,MedicationId")] Indication indication)
        {
            if (id != indication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(indication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IndicationExists(indication.Id))
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
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Id", indication.MedicationId);
            return View(indication);
        }

        // GET: Indication/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indication = await _context.Indications
                .Include(i => i.Medication)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (indication == null)
            {
                return NotFound();
            }

            return View(indication);
        }

        // POST: Indication/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var indication = await _context.Indications.FindAsync(id);
            if (indication != null)
            {
                _context.Indications.Remove(indication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IndicationExists(int id)
        {
            return _context.Indications.Any(e => e.Id == id);
        }
    }
}

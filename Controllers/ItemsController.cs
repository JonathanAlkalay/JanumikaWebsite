using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JanumikaPro.Data;
using JanumikaPro.Models;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Authorization;

namespace JanumikaPro.Controllers
{
    public class ItemsController : Controller
    {
        private readonly JanumikaProContext _context;

        public ItemsController(JanumikaProContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index1()
        {
            return View("Index", await _context.Item.Include(i => i.Category).ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        // GET: Items
        public async Task<IActionResult> Index()
        {
            var WebAppJanumikaContext = _context.Item.Include(i => i.Category);
            return View(await WebAppJanumikaContext.ToListAsync());
        }

        public async Task<IActionResult> Search2(string price, string color, string size)
        {
            try
            {
                int p = Int32.Parse(price);
                var applicationDbContext = _context.Item.Where(a => a.Color.Equals(color)
                && a.Size.Equals(size) && a.Price <= p);

                return View("searchlist2", await applicationDbContext.ToListAsync());
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        //Search Product
        public async Task<IActionResult> Search(string price, string category, string productName)
        {
            try
            {
                int p = Int32.Parse(price);
                var applicationDbContext = _context.Item.Include(a => a.Category).Where(a => a.Name.Contains(productName)
                && a.Category.Name.Equals(category) && a.Price <= p);

                return View("SearchList", await applicationDbContext.ToListAsync());
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }


        public async Task<IActionResult> SearchItems(string query)
        {
            /* LinQ
             var q= from a in _context.Item.Include(i => i.Category)
                    Where(a=> a.Name.Contains(query) || query==null)
                    orederby a.Name descending
                    select a.Name + " " + a.category;
          */

            var finalProject1Context = _context.Item.Include(i => i.Category).Where(a => a.Name.Contains(query) || query == null);
            return View("Index", await finalProject1Context.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryId");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,Name,CategoryId,Price,Image,Size,Color")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryId", item.CategoryId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryId", item.CategoryId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,Name,CategoryId,Price,Image,Size,Color")] Item item)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemId))
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
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryId", item.CategoryId);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Item.FindAsync(id);
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.ItemId == id);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Statistics()
        {
            try
            {
                //Graph-1- which user has most orders
                ICollection<Stat> stat1 = new Collection<Stat>();
                var res1 = from i in _context.User.Include(o => o.Orders)
                           where (i.Orders.Count > 0)
                           orderby (i.Orders.Count) descending
                           select i;

                foreach (var i in res1)
                {
                    stat1.Add(new Stat(i.UserName, i.Orders.Count));
                }

                ViewBag.data1 = stat1;

                //Graph-2- which Category has most items
                ICollection<Stat> stat2 = new Collection<Stat>();
                List<Item> items = _context.Item.ToList();
                List<Category> categories = _context.Category.ToList();
                var res2 = from i in items
                           join c in categories on i.CategoryId equals c.CategoryId
                           group c by c.CategoryId into G
                           select new { id = G.Key, num = G.Count() };

                var res3 = from j in res2
                           join c in categories on j.id equals c.CategoryId
                           select new { category = c.Name, count = j.num };
                foreach (var i in res3)
                {
                    if (i.count > 0)
                        stat2.Add(new Stat(i.category, i.count));
                }

                ViewBag.data2 = stat2;
                return View();
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }



    }

}
public class Stat
{
    public string Key;
    public int Values;
    public Stat(string key, int values)
    {
        Key = key;
        Values = values;
    }
}

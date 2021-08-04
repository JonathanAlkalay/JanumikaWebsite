using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JanumikaPro.Data;
using JanumikaPro.Models;
using Microsoft.AspNetCore.Authorization;

namespace JanumikaPro.Controllers
{
    public class OrdersController : Controller
    {
        private readonly JanumikaProContext _context;

        public OrdersController(JanumikaProContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        // GET: Orders
        public async Task<IActionResult> Index()
        {
            String userName = HttpContext.User.Identity.Name;
            User user = _context.User.FirstOrDefault(x => x.UserName.Equals(userName));

            var webAppJanumikaContext = _context.Order.Include(o => o.User).Where(x => x.User.UserName.Equals(userName));

            return View(await webAppJanumikaContext.ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index1()
        {
            return View("Index", await _context.Order.Include(u => u.User).ToListAsync());
        }


        // GET: Orders
        public async Task<IActionResult> UserOrders(int id)
        {
            var webAppJanumikaContext = _context.Order.Include(o => o.User).Where(or => or.UserId == id);
            return View(await webAppJanumikaContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            Cart cart = _context.Cart.Include(x => x.Items).Include(x => x.User).FirstOrDefault(x => x.Id == id);

            lock (cart)
            {
                cart.Items.RemoveAll(p => p.Id == p.Id);
                cart.TotalPrice = 0;
            }
            cart.TotalPrice = 0;

            // _context.Update(cart);
            // _context.Update(cart.User);

            _context.Attach<Cart>(cart);
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> ItemsInOrder(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Order.Include(o => o.Cart.Items).FirstOrDefault(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Password");

            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,TotalPrice,Date,UserId,Name,Country,City,Address,ZipCode,PhoneNumber")] Order order, int id)
        {

            User user = _context.User.FirstOrDefault(x => x.Id == id);
            Cart cart = _context.Cart.Include(db => db.Items).FirstOrDefault(x => x.UserId == user.Id);
            List<CartItem> cartItems = _context.CartItem.Include(i => i.Item).Where(c => cart.Items.Contains(c)).ToList();
            order.TotalPrice = cart.TotalPrice;
            order.Cart = cart;
            order.UserId = user.Id;
            order.Date = DateTime.Now;


            if (ModelState.IsValid)
            {


                _context.Add(order);
                
                foreach (CartItem CI in cartItems)
                {
                    CI.Item.Orders.Add(order);
                }
                

                cart.Items.Clear();
                cart.TotalPrice = 0;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index","Home");
            }


            ViewData["UserId"] = new SelectList(_context.User, "Id", "Password", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Password", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,TotalPrice,Date,UserId,Name,Country,City,Address,ZipCode,PhoneNumber")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Password", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
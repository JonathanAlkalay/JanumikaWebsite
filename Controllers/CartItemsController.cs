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
    public class CartItemsController : Controller
    {
        private readonly JanumikaProContext _context;

        public CartItemsController(JanumikaProContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        // GET: CartItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.CartItem.ToListAsync());
        }

        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Quantity,TotalPrice")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantity,TotalPrice")] CartItem cartItem)
        {
            if (id != cartItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.Id))
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
            return View(cartItem);
        }

        // GET: CartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.CartItem.FindAsync(id);
            _context.CartItem.Remove(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItem.Any(e => e.Id == id);
        }



        // POST: CartItems/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<double> Delete(int id)
        {
            var cartItem = await _context.CartItem.Include(c => c.Cart).FirstOrDefaultAsync(c => c.Id == id);
            cartItem.Cart.TotalPrice -= cartItem.TotalPrice;
            _context.CartItem.Remove(cartItem);
            await _context.SaveChangesAsync();
            return cartItem.Cart.TotalPrice;
        }

        // POST: CartItems/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<double[]> Update(int id, int quantity)
        {
            var cartItem = await _context.CartItem.Include(c => c.Cart).Include(p => p.Item).FirstOrDefaultAsync(c => c.Id == id);

            cartItem.Quantity += quantity;
            cartItem.Cart.TotalPrice -= cartItem.TotalPrice;
            cartItem.TotalPrice += (cartItem.Item.Price * quantity);
            cartItem.Cart.TotalPrice += cartItem.TotalPrice;

            await _context.SaveChangesAsync();

            double[] arr = { cartItem.TotalPrice, cartItem.Cart.TotalPrice };
            return arr;
        }
    }
}
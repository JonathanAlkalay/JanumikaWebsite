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
    public class CartsController : Controller
    {
        private readonly JanumikaProContext _context;

        public CartsController(JanumikaProContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var WebAppJanumikaContext = _context.Cart.Include(c => c.User);
            return View(await WebAppJanumikaContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        public IActionResult MyCart()
        {
            try
            {

                String userName = HttpContext.User.Identity.Name;
                User user = _context.User.FirstOrDefault(x => x.UserName.Equals(userName));
                Cart cart = _context.Cart.Include(db => db.Items).FirstOrDefault(x => x.UserId == user.Id);
                if (cart == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                cart.Items = _context.CartItem.Include(x => x.Item).Where(x => x.Cart.Id == cart.Id).ToList();
                // cart.Items = _context.CartItem.Where(x => x.Item.ItemId == id).ToList();


                return View(cart);
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        public async Task UpdateCartTotalPrice(double price)
        {
            String userName = HttpContext.User.Identity.Name;
            var cart = await _context.Cart.FirstOrDefaultAsync(s => s.User.UserName == userName);
            cart.TotalPrice += price;
            await _context.SaveChangesAsync();
        }

        [HttpPost, ActionName("AddToCart")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToCart(int id)// product id
        {
            try
            {
                Item Item = _context.Item.FirstOrDefault(x => x.ItemId == id);
                String userName = HttpContext.User.Identity.Name;
                User user = _context.User.FirstOrDefault(x => x.UserName.Equals(userName));
                Cart cart = _context.Cart.Include(db => db.Items).FirstOrDefault(x => x.User.Id == user.Id);

                if (cart == null)
                {
                    cart = new Cart { User = user, UserId = user.Id, TotalPrice = 0 };
                    cart.Items = new List<CartItem>();
                    _context.Cart.Add(cart);
                    await _context.SaveChangesAsync();
                }
                var c = _context.CartItem.Where(s => s.Cart == cart).Where(p => p.Item.ItemId == id).FirstOrDefault();
                if (c == null)
                {
                    CartItem cartItem = new CartItem();
                    cartItem.Quantity = 1;
                    cartItem.Item = new Item();
                    cartItem.Item=Item;
                    cartItem.TotalPrice = Item.Price * cartItem.Quantity;
                    cartItem.Cart = cart;


                    _context.CartItem.Add(cartItem);

                    cart.Items.Add(cartItem);

                    _context.Update(cart);
                    _context.Update(user);

                    await _context.SaveChangesAsync();
                    _context.Attach<CartItem>(cartItem);
                    _context.Entry(cartItem).State = EntityState.Modified;
                    
                    await UpdateCartTotalPrice(cartItem.TotalPrice);

                }
                else
                {
                    c.Quantity += 1;
                    c.TotalPrice = c.Item.Price * c.Quantity;
                    cart.Items.Add(c);


                    _context.Update(cart);
                    _context.Update(user);
                    

                    await _context.SaveChangesAsync();
                    await UpdateCartTotalPrice(c.Item.Price);

                }

                //return RedirectToAction(nameof(MyCart));
                return RedirectToAction("Index", "Home");
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        /*
       if (!(cart.Items.Contains(item) && item.Carts.Contains(cart)))
        {
            foreach(Item i in cart.Items)
            {
                if (i.ItemId == item.ItemId)
                    item.Quantity++;
            }
            user.Cart.Items.Add(item);
            item.Carts.Add(cart);
            user.Cart.TotalPrice += item.Price;
            _context.Update(cart);
            _context.Update(item);
            await _context.SaveChangesAsync();
        }
        */


        public async Task<IActionResult> RemoveProduct(int id)
        {
            try
            {
                String userName = HttpContext.User.Identity.Name;
                User user = _context.User.FirstOrDefault(x => x.UserName.Equals(userName));
                Cart cart = _context.Cart.Include(db => db.Items).FirstOrDefault(x => x.UserId == user.Id);
                CartItem CartItem = _context.CartItem.Include(i => i.Item).Where(s => s.Cart == cart).Where(p => p.Item.ItemId == id).FirstOrDefault();


                if (cart.Items.Contains(CartItem))
                    if (cart.Items.Find(x => x.Id == CartItem.Id).Quantity > 1)
                        cart.Items.Find(x => x.Id == CartItem.Id).Quantity--;
                    else
                        cart.Items.Remove(cart.Items.Find(x => x.Id == CartItem.Id));

                CartItem.TotalPrice -= CartItem.Item.Price;
                cart.TotalPrice -= CartItem.Item.Price;
                _context.Attach<Cart>(cart);
                _context.Entry(cart).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyCart));
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                String userName = HttpContext.User.Identity.Name;
                User user = _context.User.FirstOrDefault(x => x.UserName.Equals(userName));
                Cart cart = _context.Cart.Include(db => db.Items).FirstOrDefault(x => x.UserId == user.Id);
                CartItem CartItem = _context.CartItem.Include(i => i.Item).Where(s => s.Cart == cart).Where(p => p.Item.ItemId == id).FirstOrDefault();



                if (cart.Items.Contains(CartItem))
                    cart.Items.Remove(cart.Items.Find(x => x.Id == CartItem.Id));

                double price = CartItem.Item.Price * CartItem.Quantity;


                CartItem.TotalPrice -= price;
                cart.TotalPrice -= price;
                _context.Attach<Cart>(cart);
                _context.Entry(cart).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyCart));

            }
            catch { return RedirectToAction("PageNotFound", "Home"); }

        }

        public async Task<IActionResult> AddQuantity(int id)
        {

            try
            {
                String userName = HttpContext.User.Identity.Name;
                User user = _context.User.FirstOrDefault(x => x.UserName.Equals(userName));
                Cart cart = _context.Cart.Include(db => db.Items).FirstOrDefault(x => x.UserId == user.Id);
                CartItem CartItem = _context.CartItem.Include(i => i.Item).Where(s => s.Cart == cart).Where(p => p.Item.ItemId == id).FirstOrDefault();

                cart.Items.Find(x => x.Id == CartItem.Id).Quantity++;

                CartItem.TotalPrice += CartItem.Item.Price;
                cart.TotalPrice += CartItem.Item.Price;

                _context.Attach<Cart>(cart);
                _context.Entry(cart).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyCart));
            }
            catch { return RedirectToAction("PageNotFound", "Home"); }

        }






        /*


        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Password");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,TotalPrice")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Password", cart.UserId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Password", cart.UserId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,TotalPrice")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Password", cart.UserId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.Id == id);
        }
        */
    }
}
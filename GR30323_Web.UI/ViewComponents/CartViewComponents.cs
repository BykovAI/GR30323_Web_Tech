using Microsoft.AspNetCore.Mvc;
using GR30323_Web.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using GR30323_Web.UI.WebExtension;

namespace GR30323_Web.UI.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Получаем корзину из сессии
            var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            var cartSummary = new CartSummary
            {
                ItemCount = cart.CartItems.Sum(item => item.Value.Quantity),
                TotalPrice = cart.CartItems.Sum(item => item.Value.Car.Price * item.Value.Quantity)
            };
            return View(cartSummary);
        }
    }

    public class CartSummary
    {
        public int ItemCount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

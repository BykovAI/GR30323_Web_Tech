using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GR30323_Web.Domain.Models;
using GR30323_Web.UI.WebExtension;
using System.Threading.Tasks;
using GR30323_Web.Domain.Services.ProductService;

namespace GR30323_Web.UI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private Cart _cart;

        public CartController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: CartController
        public ActionResult Index()
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            return View(_cart.CartItems); // Отображаем элементы корзины
        }

        [Route("[controller]/add/{id:int}")]
        public async Task<ActionResult> Add(int id, string returnUrl)
        {
            // Получаем продукт по ID через сервис
            var data = await _productService.GetProductByIdAsync(id);
            if (data.Success)
            {
                _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
                _cart.AddToCart(data.Data); // Добавляем товар в корзину
                HttpContext.Session.Set("cart", _cart); // Сохраняем корзину в сессию
            }
            else
            {
                // Обработать ошибку, если товар не найден (например, вывести сообщение)
            }

            return Redirect(returnUrl); // Перенаправление на исходную страницу
        }

        [Route("[controller]/remove/{id:int}")]
        public ActionResult Remove(int id)
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            _cart.RemoveItems(id); // Удаляем товар из корзины
            HttpContext.Session.Set("cart", _cart); // Сохраняем обновленную корзину в сессии
            return RedirectToAction("Index"); // Перенаправляем на страницу корзины
        }

        [Route("[controller]/clear")]
        public ActionResult Clear()
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            _cart.ClearAll(); // Очищаем корзину
            HttpContext.Session.Set("cart", _cart); // Сохраняем пустую корзину в сессии
            return RedirectToAction("Index"); // Перенаправляем на страницу корзины
        }

        [Route("[controller]/checkout")]
        public ActionResult Checkout()
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            // Здесь мы можем подготовить данные для оформления заказа
            return View(_cart);
        }


        [HttpPost]
        [Route("[controller]/placeorder")]
        public ActionResult PlaceOrder(string name, string address)
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            // Здесь можно создать заказ и сохранить его в базе данных
            // Очищаем корзину после оформления заказа
            _cart.ClearAll();
            HttpContext.Session.Set<Cart>("cart", _cart);

            TempData["OrderSuccess"] = "Your order has been placed successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}

using GR30323_Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR30323_Web.Domain.Models
{


    public class Cart
    {
        public int Id { get; set; }

        /// <summary>
        /// Список объектов в корзине
        /// key - идентификатор объекта (Car.Id)
        /// </summary>
        public Dictionary<int, CartItem> CartItems { get; set; } = new();

        /// <summary>
        /// Добавить объект в корзину
        /// </summary>
        /// <param name="car">Добавляемый объект (Car)</param>
        public virtual void AddToCart(Car car)
        {
            if (CartItems.ContainsKey(car.Id))
            {
                CartItems[car.Id].Quantity++;
            }
            else
            {
                CartItems.Add(car.Id, new CartItem
                {
                    Car = car,
                    Quantity = 1
                });
            }
        }

        /// <summary>
        /// Удалить объект из корзины
        /// </summary>
        /// <param name="carId">Идентификатор автомобиля</param>
        public virtual void RemoveItems(int carId)
        {
            CartItems.Remove(carId);
        }

        /// <summary>
        /// Очистить корзину
        /// </summary>
        public virtual void ClearAll()
        {
            CartItems.Clear();
        }

        /// <summary>
        /// Количество объектов в корзине
        /// </summary>
        public int Count => CartItems.Sum(item => item.Value.Quantity);

        /// <summary>
        /// Общая стоимость всех объектов в корзине
        /// </summary>
        public decimal TotalPrice => CartItems.Sum(item => item.Value.Car.Price * item.Value.Quantity);
    }

}

﻿using ClothesShop.DAL.Data;
using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities;
using ClothesShop.Entities.Clothes;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.DAL.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader obj)
        {
            _context.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderHeaderFromDb = _context.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderHeaderFromDb != null)
            {
                orderHeaderFromDb.OrderStatus = orderStatus;

                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderHeaderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }
        public void UpdatePaymentStatus(int id, string sessionId, string paymentIntentId)
        {
            var orderHeaderFromDb = _context.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderHeaderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderHeaderFromDb.PaymentIntentId = paymentIntentId;
                orderHeaderFromDb.PaymentDate = DateTime.Now;
            }
        }
    }
}

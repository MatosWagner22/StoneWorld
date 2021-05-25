﻿using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoneWorld_DataAccess.Repository.IRepository;
using StoneWorld_Models;
using StoneWorld_Models.ViewModels;
using StoneWorld_Utility;
using StoneWorld_Utility.BrainTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneWorld.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailRepository _orderDRepo;
        private readonly IBrainTreeGate _brain;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IOrderHeaderRepository orderHRepo, IOrderDetailRepository orderDRepo, IBrainTreeGate brain)
        {
            _orderHRepo = orderHRepo;
            _orderDRepo = orderDRepo;
            _brain = brain;
        }

        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string Status = null)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHList = _orderHRepo.GetAll(),
                StatusList = WC.listStatus.ToList().Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                     Text = i,
                     Value = i
                })
            };

            if (!string.IsNullOrEmpty (searchName))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }

            if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.OrderStatus.ToLower().Contains(Status.ToLower()));
            }

            return View(orderListVM);
        }

        public IActionResult Details(int id)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _orderHRepo.FirstOrDefault(u => u.Id == id),
                OrderDetail = _orderDRepo.GetAll(o => o.OrderHeaderId == id, includeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WC.StatusinProcess;
            _orderHRepo.Save();
            TempData[WC.Success] = "Order is in Process";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderHRepo.Save();
            TempData[WC.Success] = "Order Shipped Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);

            var gateway = _brain.GetGateway();
            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);

            if (transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                // no refund
                Result<Transaction> resultVoid = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                //refund
                Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
            }

            orderHeader.OrderStatus = WC.StatusRefunded;
            _orderHRepo.Save();
            TempData[WC.Success] = "Order Cancelled Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromdb = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);

            orderHeaderFromdb.FullName = OrderVM.OrderHeader.FullName;
            orderHeaderFromdb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromdb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromdb.City = OrderVM.OrderHeader.City;
            orderHeaderFromdb.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFromdb.State = OrderVM.OrderHeader.State;
            orderHeaderFromdb.Email = OrderVM.OrderHeader.Email;

            _orderHRepo.Save();
            TempData[WC.Success] = "Order Details Updated Successfully";

            return RedirectToAction("Details","Order",new { id = orderHeaderFromdb.Id});
        }

    }
}

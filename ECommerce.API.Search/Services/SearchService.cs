﻿using ECommerce.API.Search.Interface;
using ECommerce.API.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.API.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;
        private readonly IProductsService productsService;
        private readonly ICustomersService customersService;

        public SearchService(IOrdersService ordersService, IProductsService productsService, ICustomersService customersService)
        {
            this.ordersService = ordersService;
            this.productsService = productsService;
            this.customersService = customersService;
        }

        public async Task<(bool isSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var ordersResult = await ordersService.GetOrdersAsync(customerId);
            var productsResult = await productsService.GetProductsAsync();
            var customersResult = await customersService.GetCustomerAsync(customerId);

            if (ordersResult.isSuccess)
            {
                foreach (var order in ordersResult.Orders)
                {
                    foreach (var item in order.Items)
                    {
                        item.ProductName = productsResult.isSuccess ? productsResult.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name :
                            "Product informatin is not available";
                    }
                }
                var result = new
                {
                    Customer = customersResult.isSuccess ? customersResult.Customer : new Customer { Name="Customer information is not available"},
                    Orders = ordersResult.Orders
                };
                return (true, result);
            }
            return (false, null);
        }

    }
}

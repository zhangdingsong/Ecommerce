﻿using Ecommerce.ProductCatalog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.ProductCatalog
{
    interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task AddProduct(Product product);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Threading;

namespace Ecommerce.ProductCatalog
{
    class ServiceFabricProductRepository : IProductRepository
    {
        private readonly IReliableStateManager _stateManager;
        private object cancellationToken;

        public ServiceFabricProductRepository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public async Task AddProduct(Product product)
        {
            var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("Products");

            using (var tx = _stateManager.CreateTransaction())
            {
                await products.AddOrUpdateAsync(tx, product.Id, product, (id, value) => product);

                await tx.CommitAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("Products");
            var result = new List<Product>();

            using (var tx = _stateManager.CreateTransaction())
            {
                var allProducts = await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                using (var enumerator = allProducts.GetAsyncEnumerator())
                {
                    while( await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, Product> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }

            return result;
        }
    }
}

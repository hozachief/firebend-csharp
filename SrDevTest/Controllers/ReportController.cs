using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Data;
using System.Linq;
using System.Threading.Tasks;
using Services.Models;

namespace SrDevTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        [HttpPost]
        public void Post(Product product)
        {
            AdventureWorks2019Context context = new AdventureWorks2019Context();
            var dbProducts = context.Products;

            dbProducts.Add(product);
        }

        [HttpGet]
        public ProductDto GetSingleProduct(int productId)
        {
            AdventureWorks2019Context context = new AdventureWorks2019Context();
            var dbProducts = context.Products;

            var product = dbProducts.Where(x => x.ProductId == productId).FirstOrDefault();

            ProductDto productDto = new ProductDto()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ProductNumber = product.ProductNumber,
                MakeFlag = product.MakeFlag,
                FinishedGoodsFlag = product.FinishedGoodsFlag,
                Color = product.Color,
                SafetyStockLevel = product.SafetyStockLevel,
                ReorderPoint = product.ReorderPoint,
                StandardCost = product.StandardCost,
                ListPrice = product.ListPrice,
                Size = product.Size,
                SizeUnitMeasureCode = product.SizeUnitMeasureCode,
                WeightUnitMeasureCode = product.WeightUnitMeasureCode,
                Weight = product.Weight,
                DaysToManufacture = product.DaysToManufacture,
                ProductLine = product.ProductLine,
                Class = product.Class,
                Style = product.Style,
                ProductSubcategoryId = product.ProductSubcategoryId,
                ProductModelId = product.ProductModelId,
                SellStartDate = product.SellStartDate,
                SellEndDate = product.SellEndDate,
                DiscontinuedDate = product.DiscontinuedDate,
                Rowguid = product.Rowguid,
                ModifiedDate = product.ModifiedDate
            };

            return productDto;
        }

        [HttpGet]
        [Route("GetAll")]
        public List<ProductDto> GetAllProductsByQuantity(int quantity)
        {
            List<ProductDto> productList = new List<ProductDto>();
            AdventureWorks2019Context context = new AdventureWorks2019Context();
            var dbProductInventories = context.ProductInventories;
            var dbProducts = context.Products;

            var productInventories = dbProductInventories.Where(x => x.Quantity == quantity).AsEnumerable();

            foreach (var item in productInventories)
            {
                var product = dbProducts.Where(x => x.ProductId == item.ProductId).FirstOrDefault();

                ProductDto productDto = new ProductDto()
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    ProductNumber = product.ProductNumber,
                    MakeFlag = product.MakeFlag,
                    FinishedGoodsFlag = product.FinishedGoodsFlag,
                    Color = product.Color,
                    SafetyStockLevel = product.SafetyStockLevel,
                    ReorderPoint = product.ReorderPoint,
                    StandardCost = product.StandardCost,
                    ListPrice = product.ListPrice,
                    Size = product.Size,
                    SizeUnitMeasureCode = product.SizeUnitMeasureCode,
                    WeightUnitMeasureCode = product.WeightUnitMeasureCode,
                    Weight = product.Weight,
                    DaysToManufacture = product.DaysToManufacture,
                    ProductLine = product.ProductLine,
                    Class = product.Class,
                    Style = product.Style,
                    ProductSubcategoryId = product.ProductSubcategoryId,
                    ProductModelId = product.ProductModelId,
                    SellStartDate = product.SellStartDate,
                    SellEndDate = product.SellEndDate,
                    DiscontinuedDate = product.DiscontinuedDate,
                    Rowguid = product.Rowguid,
                    ModifiedDate = product.ModifiedDate
                };

                productList.Add(productDto);
            }

            return productList;
        }

        [HttpPut]
        public void Put(Product product)
        {
            AdventureWorks2019Context context = new AdventureWorks2019Context();
            var dbProducts = context.Products;

            dbProducts.Add(product);
        }

        [HttpDelete]
        public void Delete(int productId)
        {
            AdventureWorks2019Context context = new AdventureWorks2019Context();
            var dbProducts = context.Products;

            var product = dbProducts.Where(x => x.ProductId == productId).FirstOrDefault();

            dbProducts.Remove(product);
        }

        [HttpGet]
        [Route("InventoryStockReport")]
        public InventoryStockReportDto GetInventoryStockReport(DateTime? beginDate, DateTime? endDate, int? productNumber)
        {
            InventoryStockReportDto inventoryStockReportDto = new InventoryStockReportDto {};

            AdventureWorks2019Context context = new AdventureWorks2019Context();
            var dbProductInventories = context.ProductInventories;
            var dbSalesOrderDetails = context.SalesOrderDetails;

            // TODO JOSE: There can be multiple LocationIDs...
            var productInventories = dbProductInventories
                .Where(x => x.ProductId == productNumber && (x.ModifiedDate >= beginDate && x.ModifiedDate <= endDate))
                .GroupBy(x => new { x.ProductId, x.LocationId })
                .Select(g => new { g.Key.ProductId, g.Key.LocationId, QtyOnHand = g.Sum(q => q.Quantity) })
                .FirstOrDefault();

            var salesOrderDetails = dbSalesOrderDetails
                .Where(x => x.ProductId == productNumber)
                .GroupBy(x => new { x.ProductId })
                .Select(g => new { g.Key.ProductId, QtyUsed = g.Sum(q => q.OrderQty) })
                .FirstOrDefault();

            if (productInventories != null && salesOrderDetails != null)
            {
                var consumptionQty = productInventories.QtyOnHand - salesOrderDetails.QtyUsed;

                double percentageOverage = 0;
                if (consumptionQty > salesOrderDetails.QtyUsed)
                {
                    percentageOverage = (consumptionQty / productInventories.QtyOnHand) * 100;
                }

                inventoryStockReportDto.ProductLocation = productInventories.LocationId;
                inventoryStockReportDto.ProductId = productInventories.ProductId;
                inventoryStockReportDto.QtyOnHand = productInventories.QtyOnHand;
                inventoryStockReportDto.ConsumptionQty = consumptionQty;
                inventoryStockReportDto.PercentageOfOverage = percentageOverage;
            }

            return inventoryStockReportDto;
        }
    }
}
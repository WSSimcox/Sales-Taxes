using DealerOn.SalesTaxes.Data;
using DealerOn.SalesTaxes.Models;
using DealerOn.SalesTaxes.Models.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DealerOn.SalesTaxes.Services.Tests
{
    [TestClass]
    public class TransactionServicesTest
    {
        [TestInitialize]
        public void Initialize()
        {
            var productRepo = new ProductInMemoryRepository();

            productRepo.DefaultProductFiller();
        }

        /// <summary>
        /// Test method for AddProduct()
        /// </summary>
        [TestMethod]
        public void AddProductTest()
        {
            // Initializing ProductInMemoryRepository
            var productRepo = new ProductInMemoryRepository();

            // Initializing TransactionServices
            var transaction = new TransactionServices(productRepo, CalculatorFiller());

            // Adding LineItems to transaction
            transaction.AddLineItem(productRepo.GetProducts()[0]);
            transaction.AddLineItem(productRepo.GetProducts()[1]);
            transaction.AddLineItem(productRepo.GetProducts()[2]);

            // Updating receipt
            var receipt = transaction.GenerateReceipt();

            Assert.IsTrue(receipt.LineItems?.Count == 3);
        }

        /// <summary>
        /// Test method for RemoveProduct()
        /// </summary>
        [TestMethod]
        public void RemoveProductTest()
        {
            // Initializing ProductInMemoryRepository
            var productRepo = new ProductInMemoryRepository();

            // Initializing TransactionServices
            var transaction = new TransactionServices(productRepo, CalculatorFiller());

            // Adding LineItems to transaction
            transaction.AddLineItem(productRepo.GetProducts()[0]);
            transaction.AddLineItem(productRepo.GetProducts()[1]);

            // Checking if everything is added
            Assert.IsTrue(transaction.GetAllProductCount() == 2);

            // Removing LineItem
            transaction.RemoveLineItem(productRepo.GetProducts()[1].Id);

            // Checking if LineItem was removed
            Assert.IsTrue(transaction.GetAllProductCount() == 1);
        }

        /// <summary>
        /// Test method for GenerateReceipt
        /// </summary>
        [TestMethod]
        public void GenerateReceiptTest()
        {
            // Initializing ProductInMemoryRepository
            var productRepo = new ProductInMemoryRepository();

            // Initializing TransactionServices
            var transaction = new TransactionServices(productRepo, CalculatorFiller());

            productRepo.DefaultProductFiller();

            // Creating new Products to test calculators
            var productOne = productRepo.GetProducts()[3];
            var productTwo = productRepo.GetProducts()[4];

            // Creating new LineItems
            var lineItemOne = new LineItem(productOne);
            var lineItemTwo = new LineItem(productTwo, 1);

            // Initializing ListItem list
            var list = new List<ILineItem>();

            list.Add(lineItemOne);
            list.Add(lineItemTwo);

            // Making new receipt
            var receipt = transaction.GenerateReceipt(list);

            Assert.IsTrue(receipt.TotalCost == 65.15M);
            Assert.IsTrue(receipt.TotalTax == 7.65M);
        }

        /// <summary>
        /// This helper function creates and returns an array of Calculators
        /// One being for Sales Tax and the other for Import tax
        /// </summary>
        /// <returns> Array of Calculators </returns>
        private ITaxCalculatorServices[] CalculatorFiller()
        {
            ITaxCalculatorServices[] calcArray = new ITaxCalculatorServices[2]; 

            var salesCalc = new SalesTaxCalculatorServices(new ProductTaxRepository());
            var importCalc = new ImportTaxCalculatorServices();

            calcArray[0] = salesCalc;
            calcArray[1] = importCalc;

            return calcArray;
        }

    }
}
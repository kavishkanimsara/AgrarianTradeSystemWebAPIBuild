﻿using AgrarianTradeSystemWebAPI.Data;
using AgrarianTradeSystemWebAPI.Dtos;
using AgrarianTradeSystemWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgrarianTradeSystemWebAPI.Services.ProductServices
{
	public class ProductServices : IProductServices
	{
		private readonly DataContext _context;
		private const string AzureContainerName = "products";
		private readonly IFileServices _fileServices;
		public ProductServices(DataContext context, IFileServices fileServices)
		{
			_context = context;
			_fileServices = fileServices;

		}

		public DataContext Context { get; }

		//get all products 
		public async Task<List<Product>> GetAllProduct()
		{

			var product = await _context.Products.ToListAsync(); //retrieve data from db
			return product;
		}

		public async Task<List<Product>> GetAllProductsByFarmer(string farmerId)
		{
			var products = await _context.Products.Where(p => p.FarmerID == farmerId).ToListAsync(); 
			return products;
		}


		//get sorted product list
		public async Task<List<Product>> GetAllProductsSortedByPriceAsync(bool ascending = true)
		{
			if (ascending)
			{
				return await _context.Products.OrderBy(p => p.UnitPrice).ToListAsync();
			}
			else
			{
				return await _context.Products.OrderByDescending(p => p.UnitPrice).ToListAsync();
			}
		}

		//get sorted product list with farmer's details
		public async Task<List<ProductCardDto>> GetAllProductsSortedByPrice(bool ascending = true)
		{
			var products = ascending ?
				await _context.Products.OrderBy(p => p.UnitPrice).ToListAsync() :
				await _context.Products.OrderByDescending(p => p.UnitPrice).ToListAsync();

			var productCardDtos = products.Select(p => new ProductCardDto
			{
				ProductID = p.ProductID,
				ProductTitle = p.ProductTitle,
				ProductImageUrl = p.ProductImageUrl,
				FarmerAddL3 = p.Farmer?.AddL3,
				ProductType = p.ProductType,
				Category = p.Category,
				UnitPrice = p.UnitPrice,
				AvailableStock = p.AvailableStock,
				MinimumQuantity = p.MinimumQuantity
			}).ToList();

			return productCardDtos;
		}

		//get products list with farmers details for product card
		public async Task<List<ProductCardDto>> GetAllProductsWithFarmerDetails()
		{
			var products = await _context.Products
				.Include(p => p.Farmer)
				.ToListAsync();

			var productCardDtos = products.Select(p => new ProductCardDto
			{
				ProductID = p.ProductID,
				ProductTitle = p.ProductTitle,
				ProductImageUrl = p.ProductImageUrl,
				FarmerAddL3 = p.Farmer?.AddL3 ?? "",
				ProductType = p.ProductType,
				Category = p.Category,
				UnitPrice = p.UnitPrice,
				AvailableStock = p.AvailableStock,
				MinimumQuantity = p.MinimumQuantity
			}).ToList();

			return productCardDtos;
		}

		//get single data by id
		public async Task<Product?> GetSingleProduct(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null)
				return null;
			return product;
		}

		//get single product details and farmer's details by productID for more details product
		public async Task<ProductListDto?> GetSingleProductDto(int id)
		{
			var product = await _context.Products
				.Include(p => p.Farmer)
				.FirstOrDefaultAsync(p => p.ProductID == id);

			if (product == null)
			{
				return null;
			}

			var productDto = new ProductListDto
			{
				ProductId = product.ProductID,
				ProductTitle = product.ProductTitle,
				ProductImageUrl = product.ProductImageUrl,
				FarmerFName = product.Farmer?.FirstName ?? "",
				FarmerLName = product.Farmer?.LastName ?? "",
				FarmerProfileUrl = product.Farmer?.ProfileImg ?? "",
				FarmerAddL1 = product.Farmer?.AddL1 ?? "",
				FarmerAddL2 = product.Farmer?.AddL2 ?? "",
				FarmerAddL3 = product.Farmer?.AddL3 ?? "",
				FarmerContact = product.Farmer?.PhoneNumber ?? "",
				ProductDescription = product.ProductDescription,
				ProductType = product.ProductType,
				Category = product.Category,
				UnitPrice = product.UnitPrice,
				AvailableStock = product.AvailableStock,
				MinimumQuantity = product.MinimumQuantity
			};

			return productDto;
		}

		//add products to the system
		public async Task<List<Product>> AddProduct(Product product)
		{
			_context.Products.Add(product);
			await _context.SaveChangesAsync();
			return await _context.Products.ToListAsync();
		}

		//update
		public async Task<List<Product>?> UpdateProduct(int id, [FromForm] Product request, String newFileUrl)
		{
			//find data from db
			var product = await _context.Products.FindAsync(id);
			if (product == null)
				return null;
			//get product image url Name
			var fileName = product.ProductImageUrl;

			//delete image from azure storage
			await _fileServices.Delete(fileName, AzureContainerName);

			//update database
			product.ProductTitle = request.ProductTitle;
			product.ProductDescription = request.ProductDescription;
			product.ProductImageUrl = newFileUrl;
			product.UnitPrice = request.UnitPrice;
			product.ProductType = request.ProductType;
			product.Category = request.Category;
			product.AvailableStock = request.AvailableStock;
			product.MinimumQuantity = request.MinimumQuantity;
			await _context.SaveChangesAsync();
			return await _context.Products.ToListAsync();
		}
		//update product image
		//update

		public async Task<List<Product>?> UpdateProductImage(int id, String newFileUrl)
		{
			//find data from db
			var product = await _context.Products.FindAsync(id);
			if (product == null)
				return null;
			//get product image url Name
			var fileName = product.ProductImageUrl;
			//delete image from azure storage
			await _fileServices.Delete(fileName, AzureContainerName);
			//update database
			product.ProductImageUrl = newFileUrl;
			await _context.SaveChangesAsync();
			return await _context.Products.ToListAsync();
		}
		//update without image
		public async Task<List<Product>?> UpdateProductDetails(int id, [FromForm] Product request)
		{
			//find data from db
			var product = await _context.Products.FindAsync(id);
			if (product == null)
				return null;
			//update database
			product.ProductTitle = request.ProductTitle;
			product.ProductDescription = request.ProductDescription;
			product.UnitPrice = request.UnitPrice;
			product.ProductType = request.ProductType;
			product.Category = request.Category;
			product.AvailableStock = request.AvailableStock;
			product.MinimumQuantity = request.MinimumQuantity;
			await _context.SaveChangesAsync();
			return await _context.Products.ToListAsync();
		}
		//delete
		public async Task<List<Product>?> DeleteProduct(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null)
				return null;

			//get product image url Name
			var fileName = product.ProductImageUrl;

			//delete image from azure storage
			await _fileServices.Delete(fileName, AzureContainerName);

			//remove product from database
			_context.Products.Remove(product);

			//save changes
			await _context.SaveChangesAsync();

			return await _context.Products.ToListAsync();
		}

		//search
        public async Task<List<Product>> SearchAsync(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();

            var products = await _context.Products
                .Where(p =>
                    p.ProductTitle.ToLower().Contains(searchTerm) ||
                    p.ProductDescription.ToLower().Contains(searchTerm))
                .ToListAsync();

            return products;
        }

    }
}

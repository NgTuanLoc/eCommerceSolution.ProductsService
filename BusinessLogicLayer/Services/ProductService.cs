using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services;

public class ProductService(IValidator<ProductAddRequest> productAddRequestValidator, IValidator<ProductUpdateRequest> productUpdateRequestValidator, IMapper mapper, IProductsRepository productsRepository) : IProductService
{
    private readonly IValidator<ProductAddRequest> _productAddRequestValidator = productAddRequestValidator;
    private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator = productUpdateRequestValidator;
    private readonly IMapper _mapper = mapper;
    private readonly IProductsRepository _productsRepository = productsRepository;

    public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
    {
        ArgumentNullException.ThrowIfNull(productAddRequest);

        //Validate the product using Fluent Validation
        var validationResult = await _productAddRequestValidator.ValidateAsync(productAddRequest);

        // Check the validation result
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage)); //Error1, Error2, ...
            throw new ArgumentException(errors);
        }


        //Attempt to add product
        Product productInput = _mapper.Map<Product>(productAddRequest); //Map productAddRequest into 'Product' type (it invokes ProductAddRequestToProductMappingProfile)
        Product? addedProduct = await _productsRepository.AddProduct(productInput);

        if (addedProduct == null)
        {
            return null;
        }

        ProductResponse addedProductResponse = _mapper.Map<ProductResponse>(addedProduct); //Map addedProduct into 'ProductRepsonse' type (it invokes ProductToProductResponseMappingProfile)

        return addedProductResponse;
    }


    public async Task<bool> DeleteProduct(Guid productId)
    {
        Product? existingProduct = await _productsRepository.GetProductByCondition(temp => temp.ProductId == productId);

        if (existingProduct == null)
        {
            return false;
        }

        //Attempt to delete product
        bool isDeleted = await _productsRepository.DeleteProduct(productId);
        return isDeleted;
    }


    public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> predicate)
    {
        Product? product = await _productsRepository.GetProductByCondition(predicate);
        if (product == null)
        {
            return null;
        }

        ProductResponse productResponse = _mapper.Map<ProductResponse>(product); //Invokes ProductToProductResponseMappingProfile
        return productResponse;
    }


    public async Task<List<ProductResponse?>> GetProducts()
    {
        IEnumerable<Product?> products = await _productsRepository.GetProducts();

        IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products); //Invokes ProductToProductResponseMappingProfile
        return productResponses.ToList();
    }


    public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> predicate)
    {
        IEnumerable<Product?> products = await _productsRepository.GetProductsByCondition(predicate);

        IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products); //Invokes ProductToProductResponseMappingProfile
        return productResponses.ToList();
    }


    public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
    {
        Product? existingProduct = await _productsRepository.GetProductByCondition(temp => temp.ProductId == productUpdateRequest.ProductId);


        ArgumentNullException.ThrowIfNull(existingProduct);

        //Validate the product using Fluent Validation
        var validationResult = await _productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

        // Check the validation result
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage)); //Error1, Error2, ...
            throw new ArgumentException(errors);
        }


        //Map from ProductUpdateRequest to Product type
        Product product = _mapper.Map<Product>(productUpdateRequest); //Invokes ProductUpdateRequestToProductMappingProfile

        Product? updatedProduct = await _productsRepository.UpdateProduct(product);

        ProductResponse? updatedProductResponse = _mapper.Map<ProductResponse>(updatedProduct);

        return updatedProductResponse;
    }
}

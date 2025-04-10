using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using FluentValidation.Results;

namespace ProductsMicroservice.API.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        //GET /api/products
        app.MapGet("/api/products", async (IProductService productsService) =>
        {
            // await Task.Delay(100);
            // throw new NotImplementedException();
            List<ProductResponse?> products = await productsService.GetProducts();
            return Results.Ok(products);
        });


        //GET /api/products/search/product-id/00000000-0000-0000-0000-000000000000
        app.MapGet("/api/products/search/product-id/{ProductID:guid}", async (IProductService productsService, Guid ProductID) =>
        {
            // await Task.Delay(100);
            // throw new NotImplementedException();
            ProductResponse? product = await productsService.GetProductByCondition(temp => temp.ProductId == ProductID);

            if (product == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(product);
        });


        //GET /api/products/search/xxxxxxxxxxxxxxxxxx
        app.MapGet("/api/products/search/{SearchString}", async (IProductService productsService, string SearchString) =>
        {
            List<ProductResponse?> productsByProductName = await productsService.GetProductsByCondition(temp => temp.ProductName != null && temp.ProductName.Contains(SearchString));

            List<ProductResponse?> productsByCategory = await productsService.GetProductsByCondition(temp => temp.Category != null && temp.Category.Contains(SearchString));

            var products = productsByProductName.Union(productsByCategory);

            return Results.Ok(products);
        });


        //POST /api/products
        app.MapPost("/api/products", async (IProductService productsService, IValidator<ProductAddRequest> productAddRequestValidator, ProductAddRequest productAddRequest) =>
        {
            //Validate the ProductAddRequest object using Fluent Validation
            ValidationResult validationResult = await productAddRequestValidator.ValidateAsync(productAddRequest);

            //Check the validation result
            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors
                  .GroupBy(temp => temp.PropertyName)
                  .ToDictionary(grp => grp.Key,
                    grp => grp.Select(err => err.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }


            var addedProductResponse = await productsService.AddProduct(productAddRequest);
            if (addedProductResponse != null)
                return Results.Created($"/api/products/search/product-id/{addedProductResponse.ProductId}", addedProductResponse);
            else
                return Results.Problem("Error in adding product");
        });


        //PUT /api/products
        app.MapPut("/api/products", async (IProductService productsService, IValidator<ProductUpdateRequest> productUpdateRequestValidator, ProductUpdateRequest productUpdateRequest) =>
        {
            //Validate the ProductUpdateRequest object using Fluent Validation
            ValidationResult validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

            //Check the validation result
            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors
                  .GroupBy(temp => temp.PropertyName)
                  .ToDictionary(grp => grp.Key,
                    grp => grp.Select(err => err.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }


            var updatedProductResponse = await productsService.UpdateProduct(productUpdateRequest);
            if (updatedProductResponse != null)
                return Results.Ok(updatedProductResponse);
            else
                return Results.Problem("Error in updating product");
        });


        //DELETE /api/products/xxxxxxxxxxxxxxxxxxx
        app.MapDelete("/api/products/{ProductID:guid}", async (IProductService productsService, Guid ProductID) =>
        {
            bool isDeleted = await productsService.DeleteProduct(ProductID);
            if (isDeleted)
                return Results.Ok(true);
            else
                return Results.Problem("Error in deleting product");
        });
        return app;
    }
}

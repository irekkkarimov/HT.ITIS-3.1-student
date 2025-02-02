using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Queries.GetProducts;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery ,GetProductsDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<GetProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetAllProductsAsync(cancellationToken);

            var productsDto = new GetProductsDto(products.Select(p => new GetProductDto(p.Id, p.Name)));
            
            return new Result<GetProductsDto>(productsDto, true);
        }
        catch (Exception e)
        {
            return new Result<GetProductsDto>(null, false, e.Message);
        }
    }
}
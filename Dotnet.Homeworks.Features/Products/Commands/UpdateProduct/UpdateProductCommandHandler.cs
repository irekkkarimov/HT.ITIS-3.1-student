using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Commands.UpdateProduct;

internal sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _uow;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork uow)
    {
        _productRepository = productRepository;
        _uow = uow;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = new Product
            {
                Id = request.Guid,
                Name = request.Name
            };
            await _productRepository.UpdateProductAsync(product, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return new Result(true);
        }
        catch (Exception e)
        {
            return new Result(false, e.Message);
        }
    }
}
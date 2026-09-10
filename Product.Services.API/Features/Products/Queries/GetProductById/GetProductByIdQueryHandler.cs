using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Services.API.Common;
using SNUL.Shared.Common.DTOs.Products;
using SNUL.Shared.Common.Repositories.Interfaces.Base;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;
using ProductEntity = SNUL.Shared.Domain.Models.Product;

namespace Product.Services.API.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var productRepo = _unitOfWork.GetRepository<ProductEntity, Guid>();
            var product = await productRepo.GetAll(p => !p.IsDeleted && p.Id == request.Id)
                .Select(ProductDtoMapper.Projection)
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return Result<ProductDto>.NotFound(LocalizationKeys.Product.NotFound);
            }

            return Result<ProductDto>.Success(product, LocalizationKeys.Product.Fetched);
        }
    }
}

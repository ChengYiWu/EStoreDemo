﻿using Application.Products.Queries.Models;
using MediatR;

namespace Application.Products.Queries.GetProduct;

public record GetProductQuery(
    int Id
) : IRequest<ProductResponse>;

﻿using Net.SimpleBlog.Application.Common;

namespace Net.SimpleBlog.Api.ApiModels.Response;

public class ApiResponseList<TItemData>
    : ApiResponse<IReadOnlyList<TItemData>>
{
    public ApiResponseListMeta Meta { get; private set; }
    public ApiResponseList(
        PaginatedListOutput<TItemData> paginatedListOutput
    ) : base(paginatedListOutput.Items)
    {
        Meta = new ApiResponseListMeta(
            paginatedListOutput.Page,
            paginatedListOutput.PerPage,
            paginatedListOutput.Total
        );
    }
}
using System.Text.Json;
using DatingApp.Helpers;

namespace DatingApp.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage,
        int totalItems, int totalPages)
    {
        var paginationHeader = new PaginationHeader(currentPage, totalPages, totalItems, itemsPerPage);
        response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
}
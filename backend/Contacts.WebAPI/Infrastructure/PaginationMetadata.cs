namespace Contacts.WebAPI.Infrastructure;

public class PaginationMetadata
{
    public int TotalItemCount { get; init; }
    public int TotalPageCount { get; init; }
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }

    public PaginationMetadata(int totalItemCount, int currentPage, int pageSize)
    {
        TotalItemCount = totalItemCount;
        TotalPageCount = (int) Math.Ceiling(totalItemCount / (double) pageSize);
        CurrentPage = currentPage;
        PageSize = pageSize;
    }
}
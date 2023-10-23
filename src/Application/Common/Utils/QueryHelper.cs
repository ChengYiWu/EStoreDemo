namespace Application.Common.Utils;

public static class QueryHelper
{
    private const int MAX_PAGE_SIZE = 1000;

    private const int DEFAULT_PAGE_SIZE = 10;

    private const int DEFAULT_PAGE_NUMBER = 1;

    /// <summary>
    /// 判斷是否為有效的分頁參數，若無效則回傳預設值
    /// </summary>
    /// <param name="pageSize">每頁資料筆數</param>
    /// <param name="pageNumber">第幾個分頁</param>
    /// <returns>
    ///   SQL 中的 OFFSET 參數
    ///   SQL 中的 NEXT 參數
    ///   檢查後的 PageSize
    ///   檢查後的 PageNumber
    /// </returns>
    public static (int, int, int, int) GetPagingParams(int? pageSize, int? pageNumber)
    {
        int validPageSize = pageSize is null
            ? DEFAULT_PAGE_SIZE
            : Math.Min(pageSize.Value, MAX_PAGE_SIZE);

        int validPageNumber = pageNumber is null
            ? DEFAULT_PAGE_NUMBER
            : pageNumber.Value;

        int offset = (validPageNumber - 1) * validPageSize;
        int next = validPageSize;

        return (offset, next, validPageSize, validPageNumber);
    }
}

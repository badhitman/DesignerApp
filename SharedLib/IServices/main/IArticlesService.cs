////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Articles (service)
/// </summary>
public interface IArticlesService
{
    /// <summary>
    /// Установит рубрики для статьи
    /// </summary>
    public Task<bool> UpdateRubricsForArticle(ArticleRubricsSetModel req);

    /// <summary>
    /// Создать/обновить статью
    /// </summary>
    public Task<TResponseModel<int>> ArticleCreateOrUpdate(ArticleModelDB art);

    /// <summary>
    /// Подбор статей
    /// </summary>
    public Task<TPaginationResponseModel<ArticleModelDB>> ArticlesSelect(TPaginationRequestModel<SelectArticlesRequestModel> req);

    /// <summary>
    /// Получить статьи
    /// </summary>
    public Task<ArticleModelDB[]> ArticlesRead(int[] req);
}
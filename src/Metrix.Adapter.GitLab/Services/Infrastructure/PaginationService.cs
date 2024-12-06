namespace Io.Juenger.Scrum.GitLab.Services.Infrastructure
{
    internal class PaginationService : IPaginationService
    {
        public Task<IEnumerable<T>> BrowseAllAsync<T>(Func<int, Task<List<T>>> func) => BrowseToEndAsync(1, func);

        public async Task<IEnumerable<T>> BrowseToEndAsync<T>(int firstPage, Func<int, Task<List<T>>> func)
        {
            if (firstPage < 1) firstPage = 1;
            
            var page = firstPage;
            IList<T>? pageResult;
            var totalResult = new List<T>();

            do
            {
                pageResult = await func.Invoke(page).ConfigureAwait(false);
                // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
                pageResult ??= new List<T>();
                totalResult.AddRange(pageResult);
                page++;
            } 
            while (pageResult.Any());

            return totalResult;
        }
        
        public async Task<IEnumerable<T>> BrowseAsync<T>(int firstPage, int lastPage, Func<int, Task<List<T>>> func)
        {
            if (firstPage < 1) firstPage = 1;
            
            var page = firstPage;
            IEnumerable<T> pageResult;
            var totalResult = new List<T>();

            do
            {
                pageResult = await func.Invoke(page).ConfigureAwait(false);
                totalResult.AddRange(pageResult);
                page++;
            } 
            while (pageResult.Any());

            return totalResult;
        }
    }
}
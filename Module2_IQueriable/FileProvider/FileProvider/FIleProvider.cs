namespace FileProvider
{
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;

    public class FileProvider : IQueryProvider
    {
        private readonly string _rootPath;

        public FileProvider(string rootPath)
        {
            _rootPath = rootPath;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new FileQueryable(expression, this);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>) new FileQueryable(expression, this);
        }

        public object Execute(Expression expression)
        {
            return Execute<FileInfo>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var isEnumerable = typeof(TResult).Name == "IEnumerable`1";
            return (TResult)FileProviderContext.Execute(expression, isEnumerable, _rootPath);
        }
    }
}
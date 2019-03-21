namespace FileProvider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;

    public class FileQueryable : IQueryable<FileInfo>
    {
        public FileQueryable(string rootPath)
        {
            Expression = Expression.Constant(this);
            Provider = new FileProvider(rootPath);
        }

        internal FileQueryable(Expression expression, IQueryProvider provider)
        {
            Expression = expression;
            Provider = provider;
        }

        public IEnumerator<FileInfo> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<FileInfo>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Provider.Execute<IEnumerable>(Expression).GetEnumerator();
        }

        public Type ElementType => typeof(FileInfo);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }
    }
}
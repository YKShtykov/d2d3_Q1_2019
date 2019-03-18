using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FileProvider
{
    public class FileInfoSet<T>: IQueryable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Provider.Execute<IEnumerable>(Expression).GetEnumerator();
        }

        public FileInfoSet(string rootPath)
        {
            _rootPath = rootPath;
            Expression = Expression.Constant(this);
            Provider = new FileProvider(rootPath);
        }

        internal FileInfoSet(string rootPath, Expression expression, IQueryProvider provider)
        {
            _rootPath = rootPath;
            Expression = expression;
            Provider = provider;
        }

        private string _rootPath;
        public Type ElementType => typeof(T);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }
    }
}

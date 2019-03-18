using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FileProvider
{
    public class FileProvider : IQueryProvider
    {
        private string _rootPath;

        public FileProvider(string rootPath)
        {
            this._rootPath = rootPath;
        }

        public Type Type { get; set; }
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new FileInfoSet<TElement>(_rootPath, expression, this);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var visitor = new FileInfoExpressionVisitor();

            return (TResult)visitor.GetFileInfos(expression, _rootPath);
        }
    }
}

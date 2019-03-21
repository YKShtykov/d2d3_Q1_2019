using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FileProvider
{
    class FileQueryableExpressionVisitor : ExpressionVisitor
    {
        private IQueryable<FileInfo> _files;

        public FileQueryableExpressionVisitor(IQueryable<FileInfo> files)
        {
            _files = files;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            return c.Type == typeof(FileQueryable) ? Expression.Constant(_files) : c;
        }
    }
}

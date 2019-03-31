namespace FileProvider
{
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;

    internal class FileQueryableExpressionVisitor : ExpressionVisitor
    {
        private readonly IQueryable<FileInfo> _files;

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
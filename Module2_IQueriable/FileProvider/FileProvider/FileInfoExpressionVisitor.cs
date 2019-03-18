using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FileProvider
{
    class FileInfoExpressionVisitor: ExpressionVisitor
    {
        private readonly List<FileInfo> _fileInfos = new List<FileInfo>();
        private bool _isInWhereSection = false;
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType != typeof(Queryable))
            {
                return base.VisitMethodCall(node);
            }

            switch (node.Method.Name)
            {
                case "Where":
                    _isInWhereSection = true;
                    break;
                case "Select":
                    arg = node.Arguments[1];
                    break;
                case "Count":
                    break;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.AndAlso)
            {
                Expression left = this.Visit(b.Left);
                Expression right = this.Visit(b.Right);

                //return Expression.MakeBinary(ExpressionType.OrElse, left, right, b.IsLiftedToNull, b.Method);
            }

            if (b.NodeType == ExpressionType.OrElse)
            {
                Expression left = this.Visit(b.Left);
                Expression right = this.Visit(b.Right);
            }

            if (b.NodeType == ExpressionType.Equal)
            {

                Expression left = this.Visit(b.Left);
                Expression right = this.Visit(b.Right);

                if (left.NodeType ==ExpressionType.MemberAccess && right.NodeType ==ExpressionType.Constant)
                {
                }
            }

            return base.VisitBinary(b);
        }
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Expression.Lambda<Func<FileInfo, bool>>(Visit(node.Body), Expression.Parameter(typeof(FileInfo)));
        }

        public IEnumerable<FileInfo> GetFileInfos(Expression expression, string rootPath)
        {
            var files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                _fileInfos.Add(new FileInfo(file));
            }

            Visit(expression);

            return _fileInfos;
        }
    }
}

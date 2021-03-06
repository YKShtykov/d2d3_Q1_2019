﻿namespace FileProvider
{
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;

    internal class FileProviderContext
    {
        public static object Execute(Expression expression, bool isEnumerable, string rootPath)
        {
            var files = GetFiles(rootPath);
            var visitor = new FileQueryableExpressionVisitor(files);
            var newExpression = visitor.Visit(expression);

            if (isEnumerable)
                return files.Provider.CreateQuery(newExpression);
            return files.Provider.Execute(newExpression);
        }

        private static IQueryable<FileInfo> GetFiles(string rootPath)
        {
            var filePaths = Directory.GetFiles(rootPath, "*.*",
                SearchOption.AllDirectories);

            return filePaths.Select(fp => new FileInfo(fp)).AsQueryable();
        }
    }
}
namespace _03
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    internal class Matrix
    {
        private readonly int[,] _array;

        public Matrix(int m, int n, bool fillValues = false)
        {
            M = m;
            N = n;
            _array = new int[m, n];
            if (!fillValues) return;

            var rand = new Random();

            for (var i = 0; i < m; i++)
            for (var j = 0; j < n; j++)
                _array[i, j] = rand.Next(1, 5);
        }

        public int M { get; }
        public int N { get; }

        public int this[int m, int n]
        {
            get => _array[m, n];
            set => _array[m, n] = value;
        }

        public static Matrix MultiplyByTpl(Matrix a, Matrix b)
        {
            if (a.N != b.M) throw new Exception("These matrices can not be multiplied");
            var resultMatrix = new Matrix(a.M, b.N);

            Parallel.For(0, resultMatrix.M, i =>
            {
                Parallel.For(0, resultMatrix.N, j =>
                {
                    for (var n = 0; n < a.N; n++) resultMatrix[i, j] += a[i, n] * b[n, j];
                });
            });
            return resultMatrix;
        }

        public static Matrix Multiply(Matrix a, Matrix b)
        {
            if (a.N != b.M) throw new Exception("These matrices can not be multiplied");
            var resultMatrix = new Matrix(a.M, b.N);

            for (var i = 0; i < resultMatrix.M; i++)
            for (var j = 0; j < resultMatrix.N; j++)
            for (var n = 0; n < a.N; n++)
                resultMatrix[i, j] += a[i, n] * b[n, j];

            return resultMatrix;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("\r\n");
            for (var i = 0; i < M; i++)
            {
                var arrayLine = new int[N];

                for (var j = 0; j < N; j++) arrayLine[j] = _array[i, j];

                sb.Append(string.Join(',', arrayLine));
                sb.Append("\r\n");
            }

            sb.Append("\r\n");

            return sb.ToString();
        }
    }
}
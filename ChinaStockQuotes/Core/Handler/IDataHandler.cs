using ChinaStockQuotes.Entity;

namespace ChinaStockQuotes.Core.Handler
{
    public interface IDataHandler<T> where T : TDFData
    {
        void Handle(T data);
    }

    public interface IQuotesDataHandler<T> : IDataHandler<T> where T : TDFData
    {
        int TraderInterval { get; }
    }

    public interface IQuotesMinDataHandler<T> : IDataHandler<T> where T : TDFData
    { }

    public interface IQuotesTickDataHandler<T> : IDataHandler<T> where T : TDFData
    {
    }
}


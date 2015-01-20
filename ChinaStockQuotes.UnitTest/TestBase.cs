using Rhino.Mocks;
using System;

namespace ChinaStockQuotes.UnitTest
{

    /// <summary>
    /// Base class for unit testing
    /// </summary>
    public class TestsBase : IDisposable
    {
        /// <summary>
        /// mocks repository holder
        /// </summary>
        private readonly MockRepository mocks = new MockRepository();

        /// <summary>
        /// Gets the Mock Repository.
        /// </summary>
        protected MockRepository Mocks
        {
            get
            {
                return this.mocks;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ////this.mocks.VerifyAll();
            }
        }
    }
}

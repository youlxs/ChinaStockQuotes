using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaStockQuotes.Persistance
{
    public class MarketStockQuotesRepository : Repository<ChinaStockQuotes.Entity.MarketStockQuotes, string>
    {
        public override Entity.MarketStockQuotes GetById(string id)
        {
            throw new NotImplementedException();
        }

        public override Entity.MarketStockQuotes Add(Entity.MarketStockQuotes entity)
        {
            throw new NotImplementedException();
        }

        public override void Update(Entity.MarketStockQuotes entity)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}

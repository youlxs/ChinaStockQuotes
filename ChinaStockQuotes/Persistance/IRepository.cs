using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace ChinaStockQuotes.Persistance
{
    public interface IRepository<TEntity, TIdType>
    {
        TEntity GetById(TIdType id);



        TEntity Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TIdType id);

        void AddMany(IList<TEntity> entities);

        void UpdateMany(IList<TEntity> entities);

        PetaPoco.Database GetDatabese();         
    }
}

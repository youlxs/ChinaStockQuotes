using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace ChinaStockQuotes.Persistance
{
    public abstract class Repository<TEntity, TIdType> : IRepository<TEntity, TIdType>
    {
        protected readonly Database Db;
        private const string DatabaseConnectionStringName = "DB";

        protected Repository()
        {
            this.Db = new Database(DatabaseConnectionStringName);
        }

        public abstract TEntity GetById(TIdType id);

        public abstract TEntity Add(TEntity entity);

        public abstract void Update(TEntity entity);

        public abstract void Delete(TIdType id);

        public void AddMany(IList<TEntity> entities)
        {
            if (this.Db == null)
            {
                throw new ArgumentException("PetaPoco database can not be null.");
            }

            try
            {
                this.Db.BeginTransaction();
                foreach (TEntity entity in entities)
                {
                    this.Add(entity);
                }

                this.Db.CompleteTransaction();
            }
            catch (Exception)
            {
                this.Db.AbortTransaction();
                throw;
            }
        }

        public void UpdateMany(IList<TEntity> entities)
        {
            if (this.Db == null)
            {
                throw new ArgumentException("PetaPoco database can not be null.");
            }

            try
            {
                this.Db.BeginTransaction();
                foreach (TEntity entity in entities)
                {
                    this.Update(entity);
                }

                this.Db.CompleteTransaction();
            }
            catch (Exception)
            {
                this.Db.AbortTransaction();
                throw;
            }
        }

        public Database GetDatabese()
        {
            return this.Db;
        }
    }
}

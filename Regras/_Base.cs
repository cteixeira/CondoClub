using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Data.Objects.DataClasses;

namespace CondoClub.Regras {

    internal class _Base<TEntity> where TEntity : EntityObject {

        internal virtual IEnumerable<TEntity> Lista() {
            using (BD.Context ctx = new BD.Context()) {
                return Lista(ctx);
            };
        }

        internal virtual IEnumerable<TEntity> Lista(BD.Context ctx) {
            return ctx.CreateObjectSet<TEntity>().ToList();
        }

        internal virtual TEntity Abrir(long id) {
            using (BD.Context ctx = new BD.Context()) {
                return Abrir(id, ctx);
            }
        }

        internal virtual TEntity Abrir(long id, BD.Context ctx) {
            using (System.Data.Entity.DbContext dbCtx = GetDbContext(ctx)) {
                return dbCtx.Set<TEntity>().Find(id);
            }
        }

        internal virtual void Inserir(TEntity obj) {
            using (BD.Context ctx = new BD.Context()) {
                Inserir(obj, ctx);
                ctx.SaveChanges();
            }
        }

        internal virtual void Inserir(TEntity obj, BD.Context ctx) {
            ctx.CreateObjectSet<TEntity>().AddObject(obj);
        }

        internal virtual void Actualizar(TEntity obj) {
            using (BD.Context ctx = new BD.Context()) {
                Actualizar(obj, ctx);
                ctx.SaveChanges();
            }
        }

        internal virtual void Actualizar(TEntity obj, BD.Context ctx) {
            ctx.CreateObjectSet<TEntity>().Attach(obj);
            ctx.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
        }

        internal virtual void Apagar(long id) {
            using (BD.Context ctx = new BD.Context()) {
                Apagar(id, ctx);
                ctx.SaveChanges();
            }
        }

        internal virtual void Apagar(long id, BD.Context ctx) {
            using (System.Data.Entity.DbContext dbCtx = GetDbContext(ctx)) {
                System.Data.Entity.IDbSet<TEntity> dbset = dbCtx.Set<TEntity>();
                dbset.Remove(dbset.Find(id));
            }
        }

        #region private methdos

        private System.Data.Entity.DbContext GetDbContext() {
            return new System.Data.Entity.DbContext(new BD.Context(), true);
        }

        private System.Data.Entity.DbContext GetDbContext(BD.Context ctx) {
            return new System.Data.Entity.DbContext(ctx, false);
        }

        #endregion

    }
}

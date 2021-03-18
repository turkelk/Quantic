using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quantic.Core;
using Quantic.Ef.Util;

namespace Quantic.Ef
{
    public class QuanticDbContext : DbContext
    {
        public QuanticDbContext(DbContextOptions options)
            : base(options)
        {

        }

        internal RequestContext RequestContext { get; set; }

        public override int SaveChanges()
        {
            FillBaseFields();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var parameter = Expression.Parameter(entityType.ClrType);
                var propertyMethodInfo = typeof(EF).GetMethod("Property").MakeGenericMethod(typeof(bool));
                var isDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant(BaseProperties.IsDeleted));
                BinaryExpression compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(false));
                var lambda = Expression.Lambda(compareExpression, parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            FillBaseFields();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void FillBaseFields()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues[BaseProperties.IsDeleted] = false;
                        entry.CurrentValues[BaseProperties.Guid] = (Guid)entry.CurrentValues[BaseProperties.Guid] == Guid.Empty ? Guid.NewGuid() : entry.CurrentValues[BaseProperties.Guid];
                        entry.CurrentValues[BaseProperties.IsDeleted] = false;
                        entry.CurrentValues[BaseProperties.CreatedBy] = RequestContext?.UserId ?? "";
                        entry.CurrentValues[BaseProperties.CreatedAt] = DateTime.Now.ToUniversalTime().ToUnixTimeMilliseconds();
                        entry.CurrentValues[BaseProperties.UpdatedAt] = default(long);
                        entry.CurrentValues[BaseProperties.UpdatedBy] = default;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues[BaseProperties.IsDeleted] = true;
                        entry.CurrentValues[BaseProperties.UpdatedAt] = DateTime.Now.ToUniversalTime().ToUnixTimeMilliseconds();
                        entry.CurrentValues[BaseProperties.UpdatedBy] = RequestContext?.UserId ?? "";
                        entry.Property(BaseProperties.Guid).IsModified = false;
                        entry.Property(BaseProperties.CreatedAt).IsModified = false;
                        entry.Property(BaseProperties.CreatedBy).IsModified = false;
                        break;
                    case EntityState.Modified:
                        entry.CurrentValues[BaseProperties.UpdatedAt] = DateTime.Now.ToUniversalTime().ToUnixTimeMilliseconds();
                        entry.CurrentValues[BaseProperties.UpdatedBy] = RequestContext?.UserId ?? "";
                        entry.Property(BaseProperties.Guid).IsModified = false;
                        entry.Property(BaseProperties.CreatedAt).IsModified = false;
                        entry.Property(BaseProperties.CreatedBy).IsModified = false;
                        break;
                }
            }
        }
    }
}
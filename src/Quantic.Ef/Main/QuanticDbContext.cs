using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Quantic.Ef
{
    public class QuanticDbContext : DbContext
    {
        public QuanticDbContext(DbContextOptions options)
            : base(options)
        {

        }
        public string UserId {get; private set;}

        protected void SetUser(string userId)
        {
            UserId = userId;            
        }

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
                        entry.CurrentValues[BaseProperties.Guid] = Guid.NewGuid();
                        entry.CurrentValues[BaseProperties.IsDeleted] = false;
                        entry.CurrentValues[BaseProperties.CreatedBy] = UserId;                        
                        entry.CurrentValues[BaseProperties.CreatedAt] = DateTime.Now.ToFileTime();
                        entry.CurrentValues[BaseProperties.UpdatedAt] = default;
                        entry.CurrentValues[BaseProperties.UpdatedBy] = default;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues[BaseProperties.IsDeleted] = true;
                        entry.CurrentValues[BaseProperties.UpdatedAt] = DateTime.Now.ToFileTime();
                        entry.CurrentValues[BaseProperties.UpdatedBy] = UserId;
                        entry.Property(BaseProperties.Guid).IsModified = false;
                        entry.Property(BaseProperties.CreatedAt).IsModified = false;
                        entry.Property(BaseProperties.CreatedBy).IsModified = false;                        
                        break;
                    case EntityState.Modified:
                        entry.CurrentValues[BaseProperties.UpdatedAt] = DateTime.Now.ToFileTime();
                        entry.CurrentValues[BaseProperties.UpdatedBy] = UserId;                        
                        entry.Property(BaseProperties.Guid).IsModified = false;
                        entry.Property(BaseProperties.CreatedAt).IsModified = false;
                        entry.Property(BaseProperties.CreatedBy).IsModified = false; 
                        break;
                }
            }
        }
    }
}
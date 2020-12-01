using System;

namespace Quantic.Ef.Composition
{
    public class QuanticEfConfig
    {
        internal Type DbContextType { get; private set; }
        public void AddDbContext<TDbContext>() where TDbContext : QuanticDbContext
        {
            DbContextType = typeof(TDbContext);
        }
    }
}
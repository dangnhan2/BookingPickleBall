using Microsoft.EntityFrameworkCore;

namespace PickleBall.Data
{
    public static class ApplicationModel
    {
        public static ModelBuilder AddModel (this  ModelBuilder builder)
        {
            builder.Seed();
            builder.Configuration();
            return builder;
        }
    }
}

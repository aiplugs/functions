namespace Aiplugs.Functions.Core
{
    public interface IMigration
    {
        bool NeedMigrate();
        void Migrate();
    }
}
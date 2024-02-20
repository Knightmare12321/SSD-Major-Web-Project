namespace SSD_Major_Web_Project.Interface
{
    public interface IRepository<T>
    {
        // Retrieves a record by id.
        T Get(int id);

        // Retrieves all records in the entity.
        public List<T> GetAll();
    }


}

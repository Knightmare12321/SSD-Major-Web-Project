namespace SSD_Major_Web_Project.Interface
{
    public interface IRepository<T>
    {
        // Retrieves a record by id.
        public T GetById(int id);

        // Retrieves all records in the entity.
        public List<T> GetAll();

        // Adds a new record to the entity.
        public void Add(T entity);
    }


}

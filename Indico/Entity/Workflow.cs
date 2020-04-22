namespace Indico.Entity
{
    public class Workflow
    {
        public int Id { get; }
        public string Name { get; }

        public Workflow(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}

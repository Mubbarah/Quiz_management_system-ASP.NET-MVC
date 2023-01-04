namespace Quiz_management_system.Models
{
    internal class ForeginKeyAttribute : Attribute
    {
        private string v;

        public ForeginKeyAttribute(string v)
        {
            this.v = v;
        }
    }
}
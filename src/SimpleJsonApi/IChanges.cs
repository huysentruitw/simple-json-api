namespace SimpleJsonApi
{
    internal interface IChanges
    {
        void ApplyTo(object resource);
    }
}

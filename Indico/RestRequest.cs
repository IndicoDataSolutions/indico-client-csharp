namespace Indico
{
    interface RestRequest<T>
    {
        T Call();
    }
}
